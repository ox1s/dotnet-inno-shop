using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Domain.ReviewAggregate.Events;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Events;

public class ReviewUpdatedEventHandler : INotificationHandler<ReviewUpdatedEvent>
{
    private readonly IUsersRepository _usersRepository;
    public ReviewUpdatedEventHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    public async Task Handle(ReviewUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(notification.TargetUserId, cancellationToken)
            ?? throw new EventualConsistencyException(ReviewUpdatedEvent.UserNotFound);

        var updateReviewResult = user.ApplyRatingUpdate(notification.OldRating, notification.NewRating);

        if (updateReviewResult.IsError)
        {
            throw new EventualConsistencyException(
                ReviewUpdatedEvent.ReviewUpdatingFailed,
                updateReviewResult.Errors);
        }

        await _usersRepository.UpdateAsync(user, cancellationToken);
    }
}
