using System.Data;
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

    public static DeleteReviewCommand CreateDeleteReviewCommand(
        Guid? userId = null,
        Guid? reviewId = null)
        => new DeleteReviewCommand(
            UserId: userId ?? Constants.Review.AuthorId,
            ReviewId: reviewId ?? Guid.NewGuid()
        );

    public static UpdateReviewCommand CreateUpdateReviewCommand(
        Guid? reviewId,
        Guid? userId = null,
        int? rating = null,
        string? comment = null)
        => new UpdateReviewCommand(
            Id: reviewId ?? Guid.NewGuid(),
            UserId: userId ?? Constants.Review.AuthorId,
            Rating: rating ?? Constants.Review.Rating.Value,
            Comment : comment ?? Constants.Review.Comment.Value);
}