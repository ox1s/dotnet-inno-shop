using InnoShop.SharedKernel.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserProfileActivatedEvent(Guid UserId) : IDomainEvent;

