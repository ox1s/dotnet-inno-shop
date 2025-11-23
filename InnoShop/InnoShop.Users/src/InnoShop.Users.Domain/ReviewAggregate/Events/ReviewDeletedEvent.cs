using ErrorOr;
using InnoShop.Users.Domain.Common;
using InnoShop.Users.Domain.Common.EventualConsistency;

namespace InnoShop.Users.Domain.ReviewAggregate.Events;

public record ReviewDeletedEvent(Guid Id, Guid TargetUserId, Rating Rating)
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
