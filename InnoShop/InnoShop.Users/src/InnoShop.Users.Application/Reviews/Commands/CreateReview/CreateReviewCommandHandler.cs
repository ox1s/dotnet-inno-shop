using ErrorOr;
using InnoShop.Users.Application.Common.Interfaces;
using InnoShop.Users.Domain.Common.Interfaces;
using InnoShop.Users.Domain.ReviewAggregate;
using InnoShop.Users.Domain.UserAggregate;
using MediatR;

namespace InnoShop.Users.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler(
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider) : IRequestHandler<CreateReviewCommand, ErrorOr<Review>>
{
    public async Task<ErrorOr<Review>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var targetUser = await _usersRepository.GetUserByIdAsync(request.TargetUserId, cancellationToken);
        var author = await _usersRepository.GetUserByIdAsync(request.AuthorId, cancellationToken);

        if (targetUser is null || author is null)
        {
            return UserErrors.NotFound;
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
            request.TargetUserId,
            request.AuthorId,
            rating,
            comment ?? null,
            _dateTimeProvider
        );

        if (reviewResult.IsError)
        {
            return reviewResult.Errors;
        }
        var review = reviewResult.Value;

        await _usersRepository.AddReviewAsync(review, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return review;
    }
}