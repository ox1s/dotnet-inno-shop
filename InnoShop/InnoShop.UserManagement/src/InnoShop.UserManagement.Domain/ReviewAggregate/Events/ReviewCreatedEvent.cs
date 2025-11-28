using ErrorOr;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;

namespace InnoShop.UserManagement.Domain.ReviewAggregate.Events;

public record ReviewCreatedEvent(
    Guid ReviewId,
    Guid TargetUserId,
    int RatingValue) : IDomainEvent
{
    public static readonly Error ReviewCreatingFailed = EventualConsistencyError.From(
        code: "ReviewDeletedEvent.ReviewCreatedFailed",
        description: "Review creating failed");
    public static readonly Error UserNotFound = EventualConsistencyError.From(
        code: "ReviewCreatedEvent.TargetUserNotFound",
        description: "Target user not found");
}
