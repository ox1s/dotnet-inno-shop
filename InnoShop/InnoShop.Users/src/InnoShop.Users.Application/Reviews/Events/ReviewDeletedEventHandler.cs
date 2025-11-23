using InnoShop.Users.Application.Common.Interfaces;
using InnoShop.Users.Domain.Common.EventualConsistency;
using InnoShop.Users.Domain.Common.Interfaces;
using InnoShop.Users.Domain.ReviewAggregate.Events;
using MediatR;

namespace InnoShop.Users.Application.Reviews.Events;

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
