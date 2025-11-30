using InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;
using InnoShop.UserManagement.Application.Reviews.Commands.DeleteReview;
using InnoShop.UserManagement.Application.Reviews.Commands.UpdateReview;
using InnoShop.UserManagement.TestCommon.TestConstants;

namespace InnoShop.UserManagement.TestCommon.ReviewAggregate;

public static class ReviewCommandFactory
{
    public static CreateReviewCommand CreateCreateReviewCommand(
        Guid? targetUserId = null,
        int? rating = null,
        string? comment = null
    )
    {
        return new CreateReviewCommand(
            targetUserId ?? Constants.Review.TargetUserId,
            rating ?? Constants.Review.Rating.Value,
            comment ?? Constants.Review.Comment.Value
        );
    }

    public static DeleteReviewCommand CreateDeleteReviewCommand(
        Guid? userId = null,
        Guid? reviewId = null)
    {
        return new DeleteReviewCommand(
            userId ?? Constants.Review.AuthorId,
            reviewId ?? Guid.NewGuid()
        );
    }

    public static UpdateReviewCommand CreateUpdateReviewCommand(
        Guid? reviewId,
        Guid? userId = null,
        int? rating = null,
        string? comment = null)
    {
        return new UpdateReviewCommand(
            reviewId ?? Guid.NewGuid(),
            userId ?? Constants.Review.AuthorId,
            rating ?? Constants.Review.Rating.Value,
            comment ?? Constants.Review.Comment.Value);
    }
}