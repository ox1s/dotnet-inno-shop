using InnoShop.SharedKernel.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserProfileDeactivatedEvent(Guid UserId) : IDomainEvent;