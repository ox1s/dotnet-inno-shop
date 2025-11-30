using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Contracts.Reviews;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Queries.ListReviews;

public class ListReviewsQueryHandler(IReviewsRepository reviewsRepository)
    : IRequestHandler<ListReviewsQuery, ErrorOr<List<ReviewResponse>>>
{
    public async Task<ErrorOr<List<ReviewResponse>>> Handle(ListReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await reviewsRepository.GetByTargetUserIdAsync(
            request.TargetUserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var reviewResponses = reviews.Select(r => new ReviewResponse(
            r.Id,
            r.AuthorId,
            r.TargetUserId,
            r.Rating.Value,
            r.Comment?.Value,
            r.CreatedAt,
            r.UpdatedAt)).ToList();

        return reviewResponses;
    }
}