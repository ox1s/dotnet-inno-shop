using InnoShop.UserManagement.Domain.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserDeactivatedEvent(Guid UserId) : IDomainEvent;
