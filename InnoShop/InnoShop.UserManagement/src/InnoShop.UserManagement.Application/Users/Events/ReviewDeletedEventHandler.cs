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
        var user = await _usersRepository.GetUserByIdAsync(notification.TargetUserId, cancellationToken)
            ?? throw new EventualConsistencyException(ReviewDeletedEvent.UserNotFound);

        var removeReviewResult = user.DeleteReview(notification.OldRating);

        if (removeReviewResult.IsError)
        {
            throw new EventualConsistencyException(
                ReviewDeletedEvent.ReviewNotFound,
                removeReviewResult.Errors);
        }

        await _usersRepository.UpdateAsync(user, cancellationToken);
    }
}
