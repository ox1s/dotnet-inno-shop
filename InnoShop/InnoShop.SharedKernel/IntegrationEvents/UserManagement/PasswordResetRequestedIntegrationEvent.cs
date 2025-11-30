namespace InnoShop.SharedKernel.IntegrationEvents.UserManagement;

public record PasswordResetRequestedIntegrationEvent(
    Guid UserId,
    string Email,
    string Token) : IIntegrationEvent;