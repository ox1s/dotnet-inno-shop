using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        var transaction = await dbContext.Database.BeginTransactionAsync();


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
    }
}