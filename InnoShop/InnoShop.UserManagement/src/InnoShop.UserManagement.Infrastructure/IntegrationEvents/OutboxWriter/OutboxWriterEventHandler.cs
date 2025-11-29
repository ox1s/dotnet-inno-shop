using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using InnoShop.UserManagement.Domain.UserAggregate.Events;
using InnoShop.UserManagement.Infrastructure.Persistence;
using MediatR;
using System.Text.Json;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.OutboxWriter;

public class OutboxWriterEventHandler
    : INotificationHandler<UserProfileDeactivatedEvent>,
      INotificationHandler<UserProfileActivatedEvent>,
      INotificationHandler<UserRegisteredEvent>,
      INotificationHandler<UserProfileUpdatedEvent>
{
    private readonly UserManagementDbContext _dbContext;

    public OutboxWriterEventHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UserProfileDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserProfileDeactivatedIntegrationEvent(
            UserId: notification.UserId);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
    }

    public async Task Handle(UserProfileActivatedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserProfileActivatedIntegrationEvent(
            UserId: notification.UserId);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserRegisteredIntegrationEvent(
            UserId: notification.UserId);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
    }

    public async Task Handle(UserProfileUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserProfileUpdatedIntegrationEvent(
            UserId: notification.UserId);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
    }

    private async Task AddOutboxIntegrationEventAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _dbContext.OutboxIntegrationEvents.AddAsync(new OutboxIntegrationEvent(
            EventName: integrationEvent.GetType().Name,
            EventContent: JsonSerializer.Serialize(integrationEvent)));
    }


}
