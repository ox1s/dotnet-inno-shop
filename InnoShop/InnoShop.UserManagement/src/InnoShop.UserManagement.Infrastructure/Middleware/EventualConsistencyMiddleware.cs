using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.UserManagement.Infrastructure.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate next)
{
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(
        HttpContext context,
        IPublisher publisher,
        UserManagementDbContext dbContext)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                await next(context);

                if (context.Items.TryGetValue(DomainEventsKey, out var value) &&
                    value is Queue<IDomainEvent> domainEvents)
                    while (domainEvents.TryDequeue(out var nextEvent))
                        await publisher.Publish(nextEvent);

                await transaction.CommitAsync();
            }
            catch (EventualConsistencyException)
            {
                throw;
            }
        });
    }
}