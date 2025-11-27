using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.TestCommon.TestConstants;
using InnoShop.UserManagement.TestCommon.TestUtils.Services;
using InnoShop.UserManagement.TestCommon.UserAggregate;

namespace InnoShop.UserManagement.TestCommon.ReviewAggregate;

public static class ReviewFactory
{
    public static Review CreateReview(
        User? targetUser = null,
        User? author = null,
        int? rating = null,
        string? comment = null,
        IDateTimeProvider? dateTimeProvider = null)
    {
        var target = targetUser ?? UserFactory.CreateUserWithProfile();
        var reviewer = author ?? UserFactory.CreateUserWithProfile();

        var ratingValue = Rating.Create(
            rating ?? Constants.Review.Rating.Value).Value;

        var commentText = comment ?? Constants.Review.Comment.Value;
        var commentValue = Comment.Create(commentText).Value;

        var dateProvider = dateTimeProvider ?? new TestDateTimeProvider();

        return Review.Create(
            target,
            reviewer,
            ratingValue,
            commentValue,
            dateProvider
        ).Value;
    }
}