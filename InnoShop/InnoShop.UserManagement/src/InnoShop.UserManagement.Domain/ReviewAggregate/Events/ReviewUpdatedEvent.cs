using ErrorOr;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;

namespace InnoShop.UserManagement.Domain.ReviewAggregate.Events;

public record ReviewUpdatedEvent(
    Guid ReviewId,
    Guid TargetUserId,
    int OldRating,
    int NewRating) : IDomainEvent
{
    public static readonly Error ReviewUpdatingFailed = EventualConsistencyError.From(
        "ReviewDeletedEvent.ReviewUpdatingFailed",
        "Review updating failed");

    public static readonly Error UserNotFound = EventualConsistencyError.From(
        "ReviewUpdatedEvent.TargetUserNotFound",
        "Target user not found");
}