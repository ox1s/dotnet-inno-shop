using InnoShop.SharedKernel.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserProfileUpdatedEvent(User User) : IDomainEvent;