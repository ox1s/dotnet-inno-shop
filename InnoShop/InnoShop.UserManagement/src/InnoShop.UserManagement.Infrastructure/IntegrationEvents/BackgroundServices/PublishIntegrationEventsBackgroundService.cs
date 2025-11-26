using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Throw;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.BackgroundServices;

public class PublishIntegrationEventsBackgroundService : IHostedService
{
    private Task? _doWorkTask = null;
    private PeriodicTimer? _timer = null!;
    private readonly IIntegrationEventsPublisher _integrationEventPublisher;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<PublishIntegrationEventsBackgroundService> _logger;
    private readonly CancellationTokenSource _cts;

    public PublishIntegrationEventsBackgroundService(
        IIntegrationEventsPublisher integrationEventPublisher,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<PublishIntegrationEventsBackgroundService> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _cts = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _doWorkTask = DoWorkAsync();

        return Task.CompletedTask;
    }

    private async Task DoWorkAsync()
    {
        _logger.LogInformation("Starting integration event publisher background service.");

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            try
            {
                await PublishIntegrationEventsFromDbAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while publishing integration events.");
            }
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event {EventName}. Will retry later.", outboxEvent.EventName);
            }
        }
        await dbContext.CommitChangesAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_doWorkTask is null)
        {
            return;
        }

        _cts.Cancel();
        await _doWorkTask;

        _timer?.Dispose();
        _cts.Dispose();
    }
}
