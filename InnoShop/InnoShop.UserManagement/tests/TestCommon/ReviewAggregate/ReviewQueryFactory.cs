using InnoShop.UserManagement.Application.Reviews.Queries.GetReview;

namespace InnoShop.UserManagementTestCommon.ReviewAggregate;

public static class ReviewQueryFactory
{
    public static GetReviewQuery CreateGetReviewQuery(Guid reviewId)
    {
        return new GetReviewQuery(reviewId);
    }
}