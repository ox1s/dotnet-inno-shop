using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using InnoShop.UserManagement.Domain.UserAggregate.Events;
using InnoShop.UserManagement.Infrastructure.Persistence;
using MediatR;
using System.Text.Json;
using System.Threading;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.OutboxWriter;

public class OutboxWriterEventHandler
    : INotificationHandler<UserProfileDeactivatedIntegrationEvent>,
      INotificationHandler<UserProfileActivatedIntegrationEvent>

{
    private readonly UserManagementDbContext _dbContext;

    public OutboxWriterEventHandler(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UserProfileDeactivatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserProfileDeactivatedIntegrationEvent(
            UserId: notification.UserId);

        await AddOutboxIntegrationEventAsync(integrationEvent, cancellationToken);
    }

    public async Task Handle(UserProfileActivatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserProfileActivatedIntegrationEvent(
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
