using InnoShop.SharedKernel.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserRegisteredEvent(Guid UserId) : IDomainEvent;