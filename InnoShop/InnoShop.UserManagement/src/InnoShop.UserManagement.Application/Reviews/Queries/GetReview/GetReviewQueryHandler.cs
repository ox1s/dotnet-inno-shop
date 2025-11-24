using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Queries.GetReview;

public class GetReviewQueryHandler(
    IReviewsRepository _reviewsRepository)
    : IRequestHandler<GetReviewQuery, ErrorOr<Review>>
{
    public async Task<ErrorOr<Review>> Handle(GetReviewQuery query, CancellationToken cancellationToken)
    {
        var review = await _reviewsRepository.GetByIdAsync(query.ReviewId, cancellationToken);

        if (review is null)
        {
            return ReviewErrors.NotFound;
        }

        return review;
    }
}