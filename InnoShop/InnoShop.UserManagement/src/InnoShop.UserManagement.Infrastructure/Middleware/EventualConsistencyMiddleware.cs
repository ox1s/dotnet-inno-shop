using MediatR;
using Microsoft.AspNetCore.Http;

using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Infrastructure.Persistence;

namespace InnoShop.UserManagement.Infrastructure.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate _next)
{
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(
        HttpContext context,
        IPublisher publisher,
        UserManagementDbContext dbContext)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        context.Response.OnCompleted(async () =>
        {
            try
            {
                if (context.Items.TryGetValue(DomainEventsKey, out var value) && value is Queue<IDomainEvent> domainEvents)
                {
                    while (domainEvents.TryDequeue(out var nextEvent))
                    {
                        await publisher.Publish(nextEvent);
                    }
                }

                await transaction.CommitAsync();
            }
            catch (EventualConsistencyException)
            {
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        });

        await _next(context);
    }
}
