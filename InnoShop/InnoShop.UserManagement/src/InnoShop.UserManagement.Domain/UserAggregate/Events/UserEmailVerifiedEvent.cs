using InnoShop.SharedKernel.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserEmailVerifiedEvent(Guid UserId) : IDomainEvent;