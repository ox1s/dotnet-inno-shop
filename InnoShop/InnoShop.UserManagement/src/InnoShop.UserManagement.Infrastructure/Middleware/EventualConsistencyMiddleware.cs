using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace InnoShop.UserManagement.Infrastructure.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate next)
{
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(
        HttpContext context,
        IPublisher publisher,
        UserManagementDbContext dbContext,
        ILogger<EventualConsistencyMiddleware> logger)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                await next(context);

                context.Response.OnCompleted(async () =>
                {
                    try
                    {
                        if (context.Items.TryGetValue(DomainEventsKey, out var value)
                            && value is Queue<IDomainEvent> domainEvents)
                        {
                            while (domainEvents.TryDequeue(out var nextEvent))
                            {
                                await publisher.Publish(nextEvent);
                            }
                        }

                        await transaction.CommitAsync();
                    }
                    catch (EventualConsistencyException ex)
                    {
                        logger.LogWarning(ex, "Eventual consistency exception occurred during transaction commit");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Exception occurred during transaction commit. This may include exceptions from NpgsqlRetryingExecutionStrategy after retries are exhausted.");
                    }
                    finally
                    {
                        await transaction.DisposeAsync();
                    }
                });

                // TODO : РЕШИТЬ ЭТУ ПРОБЛЕМУ С транзакцией

                // System.ObjectDisposedException: NpgsqlTransaction
                //     at Npgsql.ThrowHelper.ThrowObjectDisposedException(String objectName, Exception innerException)
                // at Npgsql.NpgsqlTransaction.CheckDisposed()
                // at Npgsql.NpgsqlTransaction.CheckReady()
                // at Npgsql.NpgsqlTransaction.Commit(Boolean async, CancellationToken cancellationToken)
                // at Microsoft.EntityFrameworkCore.Storage.RelationalTransaction.CommitAsync(CancellationToken cancellationToken)
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

    }
}