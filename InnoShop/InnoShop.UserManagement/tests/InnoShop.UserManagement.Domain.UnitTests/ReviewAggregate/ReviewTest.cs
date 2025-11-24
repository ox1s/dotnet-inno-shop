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
        var targetUserId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        Comment? comment = commentValue is not null
            ? Comment.Create(commentValue).Value
            : null;

        // Act
        var createdReviewResult = Review.Create(
            targetUserId,
            authorId,
            Constants.Review.ValidRating,
            comment,
            _dateTimeProvider);

        // Assert
        createdReviewResult.IsError.Should().BeFalse();
        var review = createdReviewResult.Value;

        review.TargetUserId.Should().Be(targetUserId);
        review.AuthorId.Should().Be(authorId);
        review.Rating.Should().Be(Constants.Review.ValidRating);
        review.Comment.Should().Be(comment);
    }




    [Fact]
    public void Create_WhenAuthorIsTarget_ShouldFail()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var createdReviewRusult = Review.Create(
            targetUserId: userId,
            authorId: userId,
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
        var review = Review.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
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

}
