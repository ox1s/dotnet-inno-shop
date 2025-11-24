using InnoShop.UserManagement.Domain.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserActivatedEvent(Guid UserId) : IDomainEvent;

