namespace InnoShop.SharedKernel.IntegrationEvents.UserManagement;

public record UserProfileDeactivatedIntegrationEvent(Guid UserId) : IIntegrationEvent;