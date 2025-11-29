using ErrorOr;
using MediatR;

using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler(
    IReviewsRepository reviewsRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<UpdateReviewCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await reviewsRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review is null) return ReviewErrors.NotFound;

        if (review.AuthorId != request.UserId) return UserErrors.NotTheReviewAuthor;

        var ratingResult = Rating.Create(request.Rating);
        if (ratingResult.IsError) return ratingResult.Errors;
        var rating = ratingResult.Value;


        Comment? comment = null;
        if (request.Comment is not null)
        {
            var commentResult = Comment.Create(request.Comment);
            if (commentResult.IsError) return commentResult.Errors;
            comment = commentResult.Value;
        }

        var reviewResult = review.Update(
            rating,
            comment ?? null,
            dateTimeProvider
        );
        if (reviewResult.IsError) return reviewResult.Errors;


        await reviewsRepository.UpdateAsync(review, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }

}
