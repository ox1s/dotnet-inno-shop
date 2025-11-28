using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Domain.ReviewAggregate.Events;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Events;

public class ReviewDeletedEventHandler(IUsersRepository usersRepository) : INotificationHandler<ReviewDeletedEvent>
{
    public async Task Handle(ReviewDeletedEvent notification, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(notification.TargetUserId, cancellationToken)
            ?? throw new EventualConsistencyException(ReviewDeletedEvent.UserNotFound);

        var deleteReviewResult = user.ApplyRatingRemoval(notification.RatingValue);

        if (deleteReviewResult.IsError)
        {
            throw new EventualConsistencyException(
                ReviewDeletedEvent.ReviewDeletingFailed,
                deleteReviewResult.Errors);
        }

        await usersRepository.UpdateAsync(user, cancellationToken);
    }
}
