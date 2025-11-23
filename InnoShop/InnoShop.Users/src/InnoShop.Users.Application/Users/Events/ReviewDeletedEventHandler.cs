using InnoShop.Users.Application.Common.Interfaces;
using InnoShop.Users.Domain.Common.EventualConsistency;
using InnoShop.Users.Domain.ReviewAggregate.Events;
using MediatR;

namespace InnoShop.Users.Application.Users.Events;

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

        var userRatingUpdatedResult = user.UpdateRating()

        if (removeReviewResult.IsError)
        {
            throw new EventualConsistencyException(
                ReviewDeletedEvent.ReviewNotFound,
                removeReviewResult.Errors);
        }

        await _usersRepository.UpdateUserRatingAsync(notification.TargetUserId);
    }
}
