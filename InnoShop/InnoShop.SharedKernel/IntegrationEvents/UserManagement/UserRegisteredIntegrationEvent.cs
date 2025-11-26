namespace InnoShop.SharedKernel.IntegrationEvents.UserManagement;

public record UserRegisteredIntegrationEvent(Guid UserId) : IIntegrationEvent;