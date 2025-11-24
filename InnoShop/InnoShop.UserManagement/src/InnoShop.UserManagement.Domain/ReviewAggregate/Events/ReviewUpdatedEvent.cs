using ErrorOr;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;

namespace InnoShop.UserManagement.Domain.ReviewAggregate.Events;

public record ReviewUpdatedEvent(
    Guid ReviewId,
    Guid TargetUserId,
    int OldRating,
    int NewRating) : IDomainEvent
{
    public static readonly Error ReviewUpdatingFailed = EventualConsistencyError.From(
        code: "ReviewDeletedEvent.ReviewUpdatingFailed",
        description: "Review updating failed");
    public static readonly Error UserNotFound = EventualConsistencyError.From(
        code: "ReviewUpdatedEvent.TargetUserNotFound",
        description: "Target user not found");
}
