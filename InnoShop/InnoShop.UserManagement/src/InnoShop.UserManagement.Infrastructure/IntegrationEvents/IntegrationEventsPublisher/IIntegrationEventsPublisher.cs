using InnoShop.SharedKernel.IntegrationEvents;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;

public interface IIntegrationEventsPublisher
{
    Task PublishEventAsync(IIntegrationEvent integrationEvent);
}
