using System.Text.Json;
using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.BackgroundServices;

public class PublishIntegrationEventsBackgroundService : BackgroundService
{
    private readonly IIntegrationEventsPublisher _integrationEventPublisher;
    private readonly ILogger<PublishIntegrationEventsBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private PeriodicTimer _timer = null!;

    public PublishIntegrationEventsBackgroundService(
        IIntegrationEventsPublisher integrationEventPublisher,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<PublishIntegrationEventsBackgroundService> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting integration event publisher background service.");

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        while (await _timer.WaitForNextTickAsync(stoppingToken))
            try
            {
                await PublishIntegrationEventsFromDbAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while publishing integration events.");
            }
    }

    private async Task PublishIntegrationEventsFromDbAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        var outboxIntegrationEvents = dbContext.OutboxIntegrationEvents.ToList();

        _logger.LogInformation("Read a total of {NumEvents} outbox integration events", outboxIntegrationEvents.Count);

        if (outboxIntegrationEvents.Count == 0) return;

        foreach (var outboxEvent in outboxIntegrationEvents)
        {
            var integrationEvent = JsonSerializer.Deserialize<IIntegrationEvent>(outboxEvent.EventContent);

            if (integrationEvent is null)
            {
                _logger.LogError("Failed to deserialize event {EventName}", outboxEvent.EventName);
                continue;
            }

            try
            {
                await _integrationEventPublisher.PublishEventAsync(integrationEvent);
                dbContext.OutboxIntegrationEvents.Remove(outboxEvent);

                _logger.LogInformation("Event {EventId} published and marked for deletion", outboxEvent.EventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event {EventName}. Will retry later.", outboxEvent.EventName);
                break;
            }
        }

        await dbContext.CommitChangesAsync();
    }
}