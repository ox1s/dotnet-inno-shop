using ErrorOr;
using MediatR;

using InnoShop.SharedKernel.Common.Interfaces;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Contracts.Reviews;

namespace InnoShop.UserManagement.Application.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler(
    IReviewsRepository reviewsRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<UpdateReviewCommand, ErrorOr<ReviewResponse>>
{
    public async Task<ErrorOr<ReviewResponse>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
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

        return new ReviewResponse(
            review.Id,
            review.AuthorId,
            review.TargetUserId,
            review.Rating.Value,
            review.Comment?.Value,
            review.CreatedAt,
            review.UpdatedAt);
    }

}
