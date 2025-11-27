using System;
using InnoShop.UserManagement.Application.Reviews.Queries;
using InnoShop.UserManagement.Application.Reviews.Queries.GetReview;
using InnoShop.UserManagementTestCommon.ReviewAggregate;

namespace InnoShop.UserManagementTestCommon.ReviewAggregate;

public static class ReviewQueryFactory
{
    public static GetReviewQuery CreateGetReviewQuery(Guid reviewId)
        => new GetReviewQuery(reviewId);
}