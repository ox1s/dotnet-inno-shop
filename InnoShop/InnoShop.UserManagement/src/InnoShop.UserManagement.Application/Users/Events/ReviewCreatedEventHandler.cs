using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Domain.ReviewAggregate.Events;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Events;

public class ReviewCreatedEventHandler : INotificationHandler<ReviewCreatedEvent>
{
    private readonly IUsersRepository _usersRepository;
    public ReviewCreatedEventHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    public async Task Handle(ReviewCreatedEvent notification, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(notification.TargetUserId, cancellationToken)
            ?? throw new EventualConsistencyException(ReviewCreatedEvent.UserNotFound);

        var createReviewResult = user.ApplyNewRating(notification.RatingValue);

        if (createReviewResult.IsError)
        {
            throw new EventualConsistencyException(
                ReviewCreatedEvent.ReviewCreatingFailed,
                createReviewResult.Errors);
        }

        await _usersRepository.UpdateAsync(user, cancellationToken);
    }
}
