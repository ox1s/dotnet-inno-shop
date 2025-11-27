using FluentAssertions;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.TestCommon.TestConstants;
using InnoShop.UserManagement.TestCommon.TestUtils.Services;
using InnoShop.UserManagement.TestCommon.UserAggregate;

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
        var targetUser = UserFactory.CreateUserWithProfile();
        var author = UserFactory.CreateUserWithProfile();

        Comment? comment = commentValue is not null
            ? Comment.Create(commentValue).Value
            : null;

        // Act
        var createdReviewResult = Review.Create(
            targetUser,
            author,
            Constants.Review.Rating,
            comment,
            _dateTimeProvider);

        // Assert
        createdReviewResult.IsError.Should().BeFalse();
        var review = createdReviewResult.Value;

        review.TargetUserId.Should().Be(targetUser.Id);
        review.AuthorId.Should().Be(author.Id);
        review.Rating.Should().Be(Constants.Review.Rating);
        review.Comment.Should().Be(comment);
    }




    [Fact]
    public void Create_WhenAuthorIsTarget_ShouldFail()
    {
        // Arrange
        var user = UserFactory.CreateUserWithProfile();

        // Act
        var createdReviewResult = Review.Create(
            targetUser: user,
            author: user,
            Constants.Review.Rating,
            Constants.Review.Comment,
            _dateTimeProvider);

        // Assert
        createdReviewResult.IsError.Should().BeTrue();
        createdReviewResult.FirstError.Should().Be(UserErrors.UserCannotWriteAReviewForThemselves);
    }

    [Fact]
    public void Update_WhenValidData_ShouldUpdateFields()
    {
        // Arrange
        var targetUser = UserFactory.CreateUserWithProfile();
        var author = UserFactory.CreateUserWithProfile();

        var review = Review.Create(
            targetUser,
            author,
            Constants.Review.Rating,
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
}


