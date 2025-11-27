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
            TargetUserId: targetUserId ?? Constants.Review.TargetUserId,
            Rating: rating ?? Constants.Review.Rating.Value,
            Comment: comment ?? Constants.Review.Comment.Value
        );
    }
    
    public static DeleteReviewCommand CreateDeleteReviewCommand(Guid reviewId)
        => new DeleteReviewCommand(reviewId);
    
    public static UpdateReviewCommand CreateUpdateReviewCommand(
        Guid? reviewId,
        int? rating = null,
        string? comment = null)
        => new UpdateReviewCommand(
            reviewId ?? Guid.NewGuid(),
            rating ?? Constants.Review.Rating.Value,
            comment ?? Constants.Review.Comment.Value);
}