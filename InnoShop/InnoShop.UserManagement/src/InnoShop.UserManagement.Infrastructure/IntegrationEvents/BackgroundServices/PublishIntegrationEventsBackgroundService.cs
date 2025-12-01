using System.Text.Json;
using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.BackgroundServices;

public class PublishIntegrationEventsBackgroundService(
    IIntegrationEventsPublisher integrationEventPublisher,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<PublishIntegrationEventsBackgroundService> logger)
    : BackgroundService
{
    private PeriodicTimer _timer = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting integration event publisher background service.");

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        while (await _timer.WaitForNextTickAsync(stoppingToken))
            try
            {
                await PublishIntegrationEventsFromDbAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while publishing integration events.");
            }
    }

    private async Task PublishIntegrationEventsFromDbAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        var outboxIntegrationEvents = dbContext.OutboxIntegrationEvents.ToList();

        logger.LogInformation("Read a total of {NumEvents} outbox integration events", outboxIntegrationEvents.Count);

        foreach (var outboxEvent in outboxIntegrationEvents)
        {
            var integrationEvent = JsonSerializer.Deserialize<IIntegrationEvent>(outboxEvent.EventContent);

            if (integrationEvent is null)
            {
                logger.LogError("Failed to deserialize event {EventName}", outboxEvent.EventName);
                continue;
            }

            try
            {
                await integrationEventPublisher.PublishEventAsync(integrationEvent);
                dbContext.OutboxIntegrationEvents.Remove(outboxEvent);

                logger.LogInformation("Event {EventId} published and marked for deletion", outboxEvent.EventName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish event {EventName}. Will retry later.", outboxEvent.EventName);
                break;
            }
        }

        await dbContext.CommitChangesAsync();
    }
}