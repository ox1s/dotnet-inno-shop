namespace InnoShop.SharedKernel.IntegrationEvents.UserManagement;

public record UserProfileUpdatedIntegrationEvent(Guid UserId) : IIntegrationEvent;
