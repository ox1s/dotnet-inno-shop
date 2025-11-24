using FluentAssertions;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.TestCommon.TestConstants;
using InnoShop.UserManagement.TestCommon.TestUtils.Services;

namespace InnoShop.UserManagement.Domain.UnitTests.ReviewAggregate;

public class ReviewTest
{
    private readonly IDateTimeProvider _dateTimeProvider = new TestDateTimeProvider();

    [Theory]
    [InlineData("Ок.")]
    [InlineData(null)]
    public void CreateReview_WhenValidData_ShouldSuccess(string? commentValue)
    {
        // Arrange
        var targetUser = CreateUserWithProfile();
        var author = CreateUserWithProfile();

        Comment? comment = commentValue is not null
            ? Comment.Create(commentValue).Value
            : null;

        // Act
        var createdReviewResult = Review.Create(
            targetUser,
            author,
            Constants.Review.ValidRating,
            comment,
            _dateTimeProvider);

        // Assert
        createdReviewResult.IsError.Should().BeFalse();
        var review = createdReviewResult.Value;

        review.TargetUserId.Should().Be(targetUser.Id);
        review.AuthorId.Should().Be(author.Id);
        review.Rating.Should().Be(Constants.Review.ValidRating);
        review.Comment.Should().Be(comment);
    }




    [Fact]
    public void Create_WhenAuthorIsTarget_ShouldFail()
    {
        // Arrange
        var user = CreateUserWithProfile();

        // Act
        var createdReviewRusult = Review.Create(
            targetUser: user,
            author: user,
            Constants.Review.ValidRating,
            Constants.Review.Comment,
            _dateTimeProvider);

        // Assert
        createdReviewRusult.IsError.Should().BeTrue();
        createdReviewRusult.FirstError.Should().Be(UserErrors.UserCannotWriteAReviewForThemselves);
    }

    [Fact]
    public void Update_WhenValidData_ShouldUpdateFields()
    {
        // Arrange
        var targetUser = CreateUserWithProfile();
        var author = CreateUserWithProfile();

        var review = Review.Create(
            targetUser,
            author,
            Constants.Review.ValidRating,
            Constants.Review.Comment,
            _dateTimeProvider).Value;

        var newRating = Rating.Create(5).Value;
        var newComment = Comment.Create("Новый коммент").Value;

        // Act
        var updatedReviewResult = review.Update(newRating, newComment, _dateTimeProvider);

        // Assert
        updatedReviewResult.IsError.Should().BeFalse();
        review.Rating.Should().Be(newRating);
        review.Comment.Should().Be(newComment);
    }
    private static User CreateUserWithProfile()
    {
        var user = User.CreateUser(
            Constants.User.Email,
            Constants.User.PasswordHash
        );

        var profile = UserProfile.Create(
            Constants.UserProfile.FirstName,
            Constants.UserProfile.LastName,
            Constants.UserProfile.AvatarUrl,
            Constants.UserProfile.ValidPhoneNumberBelarus,
            Constants.UserProfile.ValidLocationBelarus
        ).Value;

        user.CreateUserProfile(profile);

        return user;
    }
}


