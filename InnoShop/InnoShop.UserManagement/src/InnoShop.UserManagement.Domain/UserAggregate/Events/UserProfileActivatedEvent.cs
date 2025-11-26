using InnoShop.UserManagement.Domain.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserProfileActivatedEvent(Guid UserId) : IDomainEvent;

