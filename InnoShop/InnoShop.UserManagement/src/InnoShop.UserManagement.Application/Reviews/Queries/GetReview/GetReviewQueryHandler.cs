using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Contracts.Reviews;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Queries.GetReview;

public class GetReviewQueryHandler(
    IReviewsRepository reviewsRepository)
    : IRequestHandler<GetReviewQuery, ErrorOr<ReviewResponse>>
{
    public async Task<ErrorOr<ReviewResponse>> Handle(GetReviewQuery query, CancellationToken cancellationToken)
    {
        var review = await reviewsRepository.GetByIdAsync(query.ReviewId, cancellationToken);

        if (review is null) return ReviewErrors.NotFound;

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