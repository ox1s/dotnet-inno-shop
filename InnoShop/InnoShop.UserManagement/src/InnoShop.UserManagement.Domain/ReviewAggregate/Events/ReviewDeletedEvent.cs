using ErrorOr;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;

namespace InnoShop.UserManagement.Domain.ReviewAggregate.Events;

public record ReviewDeletedEvent(
    Guid ReviewId,
    Guid TargetUserId,
    int RatingValue) : IDomainEvent
{
    public static readonly Error ReviewDeletingFailed = EventualConsistencyError.From(
        "ReviewDeletedEvent.ReviewDeletingFailed",
        "Review deleting failed");

    public static readonly Error UserNotFound = EventualConsistencyError.From(
        "ReviewDeletedEvent.TargetUserNotFound",
        "Target user not found");
}