using System.Text.Json;
using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using InnoShop.UserManagement.Domain.UserAggregate.Events;
using InnoShop.UserManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.OutboxWriter;

public class OutboxWriterEventHandler
    : INotificationHandler<UserProfileDeactivatedEvent>,
        INotificationHandler<UserProfileActivatedEvent>,
        INotificationHandler<UserRegisteredEvent>,
        INotificationHandler<UserProfileUpdatedEvent>,
        INotificationHandler<PasswordResetRequestedEvent>
{
    private readonly UserManagementDbContext _dbContext;
    private readonly ILogger<OutboxWriterEventHandler> _logger;

    public OutboxWriterEventHandler(
        UserManagementDbContext dbContext,
        ILogger<OutboxWriterEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(PasswordResetRequestedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new PasswordResetRequestedIntegrationEvent(
            notification.UserId,
            notification.Email,
            notification.Token);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
    }

    public async Task Handle(UserProfileActivatedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserProfileActivatedIntegrationEvent(
            notification.UserId);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
    }

    public async Task Handle(UserProfileDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Writing UserProfileDeactivatedIntegrationEvent to outbox for user {UserId}", notification.UserId);
        
        var integrationEvent = new UserProfileDeactivatedIntegrationEvent(
            notification.UserId);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
        
        _logger.LogInformation("UserProfileDeactivatedIntegrationEvent written to outbox for user {UserId}", notification.UserId);
    }

    public async Task Handle(UserProfileUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.User?.UserProfile == null)
            throw new ArgumentNullException(nameof(notification.User.UserProfile));

        var integrationEvent = new UserProfileUpdatedIntegrationEvent(
            notification.User.Id,
            notification.User.UserProfile.FirstName.Value,
            notification.User.UserProfile.LastName.Value,
            notification.User.UserProfile.AvatarUrl.Value,
            notification.User.AverageRating,
            notification.User.ReviewCount);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserRegisteredIntegrationEvent(
            notification.UserId);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
    }

    private async Task AddOutboxIntegrationEventAsync(IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var outboxEvent = new OutboxIntegrationEvent(
            integrationEvent.GetType().Name,
            JsonSerializer.Serialize(integrationEvent));
        
        await _dbContext.OutboxIntegrationEvents.AddAsync(outboxEvent, cancellationToken);
        
        _logger.LogInformation("Added integration event {EventType} to outbox (EventName: {EventName}). ChangeTracker state: {State}", 
            integrationEvent.GetType().Name, 
            outboxEvent.EventName,
            _dbContext.Entry(outboxEvent).State);
    }
}