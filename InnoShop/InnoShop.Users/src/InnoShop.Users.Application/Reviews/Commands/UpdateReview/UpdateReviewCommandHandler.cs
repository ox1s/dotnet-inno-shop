using ErrorOr;
using MediatR;

using InnoShop.Users.Application.Common.Interfaces;
using InnoShop.Users.Domain.Common.Interfaces;
using InnoShop.Users.Domain.ReviewAggregate;
using InnoShop.Users.Domain.UserAggregate;

namespace InnoShop.Users.Application.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler(
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider) : IRequestHandler<UpdateReviewCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _usersRepository.GetReviewByIdAsync(request.Id, cancellationToken);

        if (review is null)
        {
            return ReviewErrors.NotFound;
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

        var reviewResult = review.Update(
            rating,
            comment ?? null,
            _dateTimeProvider
        );

        if (reviewResult.IsError)
        {
            return reviewResult.Errors;
        }

        await _usersRepository.UpdateReviewAsync(review, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }

}
