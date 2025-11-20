using InnoShop.Users.Domain.Common;

namespace InnoShop.Users.Domain.UserAggregate.Events;

public record UserCreatedEvent(Guid UserId) : IDomainEvent;
