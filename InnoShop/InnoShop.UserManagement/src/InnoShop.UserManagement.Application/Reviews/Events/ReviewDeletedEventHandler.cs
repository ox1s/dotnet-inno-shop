using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate.Events;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Events;

public class ReviewDeletedEventHandler : INotificationHandler<ReviewDeletedEvent>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReviewDeletedEventHandler(IUsersRepository usersRepository, IDateTimeProvider dateTimeProvider)
    {
        _usersRepository = usersRepository;
        _dateTimeProvider = dateTimeProvider;
    }
    public async Task Handle(ReviewDeletedEvent notification, CancellationToken cancellationToken)
    {
        var review = await _usersRepository.GetReviewByIdAsync(notification.Id, cancellationToken)
            ?? throw new EventualConsistencyException(ReviewDeletedEvent.ReviewNotFound);

        var removeReviewResult = review.Delete(_dateTimeProvider);

        if (removeReviewResult.IsError)
        {
            throw new EventualConsistencyException(
                ReviewDeletedEvent.ReviewNotFound,
                removeReviewResult.Errors);
        }

        await _usersRepository.UpdateReviewAsync(review);
    }
}
