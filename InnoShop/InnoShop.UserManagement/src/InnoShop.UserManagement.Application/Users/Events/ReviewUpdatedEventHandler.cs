using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Domain.ReviewAggregate.Events;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Events;

public class ReviewUpdatedEventHandler(
    IUsersRepository usersRepository)
    : INotificationHandler<ReviewUpdatedEvent>
{
    public async Task Handle(ReviewUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(notification.TargetUserId, cancellationToken)
            ?? throw new EventualConsistencyException(ReviewUpdatedEvent.UserNotFound);

        var updateReviewResult = user.ApplyRatingUpdate(notification.OldRating, notification.NewRating);

        if (updateReviewResult.IsError)
        {
            throw new EventualConsistencyException(
                ReviewUpdatedEvent.ReviewUpdatingFailed,
                updateReviewResult.Errors);
        }

        await usersRepository.UpdateAsync(user, cancellationToken);
    }
}
