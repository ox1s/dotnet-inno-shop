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
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                await next(context);

                if (context.Items.TryGetValue(DomainEventsKey, out var value) &&
                    value is Queue<IDomainEvent> domainEvents)
                {
                    var eventCount = domainEvents.Count;
                    logger.LogInformation("Publishing {EventCount} queued domain events", eventCount);
                    
                    var eventList = new List<IDomainEvent>();
                    while (domainEvents.TryDequeue(out var nextEvent))
                    {
                        eventList.Add(nextEvent);
                        logger.LogInformation("Publishing domain event: {EventType}", nextEvent.GetType().Name);
                        await publisher.Publish(nextEvent);
                    }
                    
                    // Save outbox entries that were added by event handlers
                    var outboxCountBefore = await dbContext.OutboxIntegrationEvents.CountAsync();
                    logger.LogInformation("Saving outbox entries after publishing {EventCount} domain events. Outbox count before save: {OutboxCount}", 
                        eventList.Count, outboxCountBefore);
                    var savedCount = await dbContext.SaveChangesAsync();
                    var outboxCountAfter = await dbContext.OutboxIntegrationEvents.CountAsync();
                    logger.LogInformation("Saved {SavedCount} changes. Outbox count after save: {OutboxCount}", 
                        savedCount, outboxCountAfter);
                }
                else
                {
                    logger.LogDebug("No domain events found in context items for offline processing");
                }

                await transaction.CommitAsync();
            }
            catch (EventualConsistencyException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}