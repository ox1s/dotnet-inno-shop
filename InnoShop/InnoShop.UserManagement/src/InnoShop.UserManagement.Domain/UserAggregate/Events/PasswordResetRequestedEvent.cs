using InnoShop.SharedKernel.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record PasswordResetRequestedEvent(
    Guid UserId,
    string Email,
    string Token) : IDomainEvent;
