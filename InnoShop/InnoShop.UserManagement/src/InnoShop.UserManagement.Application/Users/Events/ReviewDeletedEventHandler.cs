using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Domain.ReviewAggregate.Events;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Events;

public class ReviewDeletedEventHandler : INotificationHandler<ReviewDeletedEvent>
{
    private readonly IUsersRepository _usersRepository;
    public ReviewDeletedEventHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    public async Task Handle(ReviewDeletedEvent notification, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(notification.TargetUserId, cancellationToken)
            ?? throw new EventualConsistencyException(ReviewDeletedEvent.UserNotFound);

        var deleteReviewResult = user.ApplyRatingRemoval(notification.RatingValue);

        if (deleteReviewResult.IsError)
        {
            throw new EventualConsistencyException(
                ReviewDeletedEvent.ReviewDeletingFailed,
                deleteReviewResult.Errors);
        }

        await _usersRepository.UpdateAsync(user, cancellationToken);
    }
}
