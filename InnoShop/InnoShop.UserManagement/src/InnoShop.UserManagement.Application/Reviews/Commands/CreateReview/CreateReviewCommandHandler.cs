using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler(
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    IReviewsRepository _reviewsRepository,
    IDateTimeProvider _dateTimeProvider) : IRequestHandler<CreateReviewCommand, ErrorOr<Review>>
{
    public async Task<ErrorOr<Review>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var targetUser = await _usersRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        var author = await _usersRepository.GetByIdAsync(request.AuthorId, cancellationToken);

        if (targetUser is null || author is null)
        {
            return UserErrors.UserNotFound;
        }

        var ratingResult = Rating.Create(request.Rating);
        if (ratingResult.IsError)
        {
            return ratingResult.Errors;
        }
        var rating = ratingResult.Value;

        Comment? comment = null;
        if (request.Comment is not null)
        {
            var commentResult = Comment.Create(request.Comment);
            if (commentResult.IsError)
            {
                return commentResult.Errors;
            }
            comment = commentResult.Value;
        }

        var reviewResult = Review.Create(
            targetUser,
            author,
            rating,
            comment ?? null,
            _dateTimeProvider
        );

        if (reviewResult.IsError)
        {
            return reviewResult.Errors;
        }
        var review = reviewResult.Value;

        await _reviewsRepository.AddReviewAsync(review, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return review;
    }
}