namespace InnoShop.SharedKernel.IntegrationEvents.UserManagement;

public record UserProfileUpdatedIntegrationEvent(
    Guid UserId,
    string FirstName,
    string LastName,
    string? AvatarUrl,
    double Rating) : IIntegrationEvent;
