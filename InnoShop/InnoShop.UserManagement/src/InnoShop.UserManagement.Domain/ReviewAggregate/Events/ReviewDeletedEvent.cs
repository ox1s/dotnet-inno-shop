using ErrorOr;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;

namespace InnoShop.UserManagement.Domain.ReviewAggregate.Events;

public record ReviewDeletedEvent(Guid Id, Guid TargetUserId, int OldRating)
    : IDomainEvent
{
    public static readonly Error ReviewNotFound = EventualConsistencyError.From(
        code: "ReviewDeletedEvent.ReviewNotFound",
        description: "Review not found");

    public static readonly Error ReviewDeleteFailed = EventualConsistencyError.From(
        code: "ReviewDeletedEvent.ReviewDeleteFailed",
        description: "Deleting review failed");

    public static readonly Error UserNotFound = EventualConsistencyError.From(
        code: "ReviewDeletedEvent.TargetUserNotFound",
        description: "Target user not found");
}
