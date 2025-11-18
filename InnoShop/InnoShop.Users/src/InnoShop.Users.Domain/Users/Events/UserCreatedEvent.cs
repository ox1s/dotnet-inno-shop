using InnoShop.Users.Domain.Common;

namespace InnoShop.Users.Domain.Users.Events;

public record UserCreatedEvent(Guid UserId) : IDomainEvent;
