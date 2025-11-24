using InnoShop.SharedKernel.IntegrationEvents;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEventsPublisher;

public interface IIntegrationEventsPublisher
{
    public void PublishEvent(IIntegrationEvent integrationEvent);
}
