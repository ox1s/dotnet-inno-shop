using InnoShop.SharedKernel.Common;

namespace InnoShop.UserManagement.Domain.UserAggregate.Events;

public record UserRatingUpdatedEvent(Guid UserId) : IDomainEvent;