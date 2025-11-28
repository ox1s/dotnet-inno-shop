using InnoShop.SharedKernel.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserProfileUpdatedEvent(Guid UserId) : IDomainEvent;
