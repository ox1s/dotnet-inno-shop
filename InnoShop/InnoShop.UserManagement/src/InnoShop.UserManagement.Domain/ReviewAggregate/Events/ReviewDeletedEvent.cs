using ErrorOr;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;

namespace InnoShop.UserManagement.Domain.ReviewAggregate.Events;

public record ReviewDeletedEvent(
    Guid ReviewId,
    Guid TargetUserId,
    int RatingValue) : IDomainEvent
{
    public static readonly Error ReviewDeletingFailed = EventualConsistencyError.From(
        code: "ReviewDeletedEvent.ReviewDeletingFailed",
        description: "Review deleting failed");

    public static readonly Error UserNotFound = EventualConsistencyError.From(
        code: "ReviewDeletedEvent.TargetUserNotFound",
        description: "Target user not found");
}
