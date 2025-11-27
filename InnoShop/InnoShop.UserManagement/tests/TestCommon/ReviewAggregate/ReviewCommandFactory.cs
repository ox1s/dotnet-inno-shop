using InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;
using InnoShop.UserManagement.TestCommon.TestConstants;

namespace InnoShop.UserManagementTestCommon.ReviewAggregate;

public static class ReviewCommandFactory
{
    public static CreateReviewCommand CreateCreateReviewCommand(
        Guid? targetUserId = null,
        int rating = Constants.Review.RatingInt,
        string? comment = Constants.Review.CommentStr
    )
    {
        return new CreateReviewCommand(
            TargetUserId: targetUserId ?? Constants.Review.TargetUserId,
            Rating: rating,
            Comment: comment
        );
    }
}