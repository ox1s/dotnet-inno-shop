using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Queries.GetReviews;

public class GetReviewsQueryHandler(IReviewsRepository reviewsRepository)
    : IRequestHandler<GetReviewsQuery, ErrorOr<List<Review>>>
{
    public async Task<ErrorOr<List<Review>>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await reviewsRepository.GetByTargetUserIdAsync(
            request.TargetUserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        return reviews;
    }
}
