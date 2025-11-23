using ErrorOr;
using FluentAssertions;
using InnoShop.Users.Domain.Common.Interfaces;
using InnoShop.Users.Domain.UserAggregate;
using InnoShop.Users.TestCommon.TestConstants;
using InnoShop.Users.TestCommon.TestUtils.Services;
using InnoShop.Users.TestCommon.UserAggregate;

namespace InnoShop.Users.Domain.UnitTests;

public class UserTests
{
    private readonly IDateTimeProvider _dateTimeProvider = new TestDateTimeProvider();

    #region UserProfile Tests

    [Fact]
    public void CreateUserProfile_WhenValidData_ShouldCreateProfile()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();
        var p = UserProfileFactory.CreateValidParams();

        // Act
        var createUserResult = user.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        // Assert
        createUserResult.IsError.Should().BeFalse();
        user.UserProfile.Should().NotBeNull();
        user.UserProfile!.FirstName.Should().Be(Constants.UserProfile.FirstName);
        user.UserProfile.Location.Country.Should().Be(Country.Belarus);
    }

    [Fact]
    public void CreateUserProfile_WhenProfileAlreadyExists_ShouldFail()
    {
        // Arrange
        var user = CreateUserWithProfile();
        var p = UserProfileFactory.CreateValidParams();

        // Act
        var addUserProfile2Result = user.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        // Assert
        addUserProfile2Result.IsError.Should().BeTrue();
        addUserProfile2Result.FirstError.Should().Be(UserErrors.CannotCreateMoreThanOneProfile);
    }

    [Fact]
    public void CreateUserProfile_WhenLocationIsNotBelarus_ShouldFail()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();
        var p = UserProfileFactory.CreateValidParams() with
        {
            Country = Constants.UserProfile.UsaCountryName,
            PhoneNumber = Constants.UserProfile.ValidPhoneNumberUsa
        };

        // Act
        var createUserResult = user.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        // Assert
        createUserResult.IsError.Should().BeTrue();
        createUserResult.FirstError.Should().Be(UserErrors.UserProfileMustBeInAllowedCountry);
        user.UserProfile.Should().BeNull();
    }

    [Fact]
    public void CreateUserProfile_WhenPhoneNumberIsInvalid_ShouldFail()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();
        var p = UserProfileFactory.CreateValidParams() with
        {
            PhoneNumber = "123"
        };

        // Act
        var addUserProfileResult = user.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        // Assert
        addUserProfileResult.IsError.Should().BeTrue();
        addUserProfileResult.FirstError.Code.Should().Contain("PhoneNumber");
        user.UserProfile.Should().BeNull();
    }

    [Fact]
    public void UpdateUserProfile_WhenProfileDoesNotExist_ShouldFail()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();
        var p = UserProfileFactory.CreateValidParams();

        // Act
        var updateUserProfileResult = user.UpdateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        // Assert
        updateUserProfileResult.IsError.Should().BeTrue();
        updateUserProfileResult.FirstError.Should().Be(UserErrors.UserMustCreateAUserProfile);
        user.UserProfile.Should().BeNull();
    }

    [Fact]
    public void UpdateUserProfile_WhenNewLocationIsNotBelarus_ShouldFail()
    {
        // Arrange
        var user = CreateUserWithProfile();
        var p = UserProfileFactory.CreateValidParams() with
        {
            Country = Constants.UserProfile.UsaCountryName
        };

        // Act
        var updateUserProfileResult = user.UpdateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        // Assert
        updateUserProfileResult.IsError.Should().BeTrue();
        updateUserProfileResult.FirstError.Should().Be(UserErrors.UserProfileMustBeInAllowedCountry);
        user.UserProfile.Should().NotBeNull();
        user.UserProfile!.Location.Country.Should().Be(Country.Belarus);
    }
    [Fact]
    public void UpdateUserProfile_WhenNewPhoneIsNotFromBelarus_ShouldFail()
    {
        // Arrange
        var user = CreateUserWithProfile();
        var p = UserProfileFactory.CreateValidParams() with
        {
            PhoneNumber = Constants.UserProfile.ValidPhoneNumberUsa
        };

        // Act
        var updateUserProfileResult = user.UpdateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        // Assert
        updateUserProfileResult.IsError.Should().BeTrue();

        updateUserProfileResult.FirstError.Code.Should().Be("PhoneNumber.WrongCountry");
        user.UserProfile.Should().NotBeNull();
        user.UserProfile!.Location.Country.Should().Be(Country.Belarus);
    }

    #endregion

    #region Review Tests

    [Fact]
    public void CreateReview_WhenValidData_ShouldSuccess()
    {
        // Arrange
        var targetUser = CreateUserWithProfile();
        var authorId = Guid.NewGuid();

        // Act
        var createdReviewResult = targetUser.CreateReview(
            authorId,
            Constants.Review.ValidRating,
            Constants.Review.Comment,
            _dateTimeProvider);

        // Assert
        createdReviewResult.IsError.Should().BeFalse();
        targetUser.Reviews.Should().HaveCount(1);

        var review = targetUser.Reviews.First();

        review.Rating.Value.Should().Be(Constants.Review.ValidRating);
        review.Comment?.Value.Should().Be(Constants.Review.Comment);
        review.AuthorId.Should().Be(authorId);
    }

    [Fact]
    public void CreateReview_WithoutComment_ShouldSuccess()
    {
        // Arrange
        var targetUser = CreateUserWithProfile();
        var authorId = Guid.NewGuid();

        // Act
        var createdReviewResult = targetUser.CreateReview(
            authorId,
            Constants.Review.ValidRating,
            null,
            _dateTimeProvider);

        // Assert
        createdReviewResult.IsError.Should().BeFalse();
        targetUser.Reviews.Should().HaveCount(1);

        var review = targetUser.Reviews.First();

        review.Comment.Should().BeNull();
        review.Rating.Value.Should().Be(Constants.Review.ValidRating);
        review.Comment?.Value.Should().Be(Constants.Review.Comment);
        review.AuthorId.Should().Be(authorId);
    }

    [Fact]
    public void UpdateReview_WhenValid_ShouldUpdate()
    {
        // Arrange
        var targetUser = CreateUserWithProfile();
        var authorId = Guid.NewGuid();

        targetUser.CreateReview(
            authorId,
            3,
            "Старый коммент",
            _dateTimeProvider);

        var reviewId = targetUser.Reviews.First().Id;

        // Act
        var updatedReviewResult = targetUser.UpdateReview(
            reviewId,
            authorId,
            4,
            "Новый коммент",
            _dateTimeProvider);

        // Assert
        updatedReviewResult.IsError.Should().BeFalse();

        var updatedReview = targetUser.Reviews.First();

        updatedReview.Rating.Value.Should().Be(4);
        updatedReview.Comment?.Value.Should().Be("Новый коммент");
    }

    [Fact]
    public void UpdateReview_WhenNotAuthor_ShouldFail()
    {
        // Arrange
        var targetUser = CreateUserWithProfile();
        var authorId = Guid.NewGuid();

        targetUser.CreateReview(
            authorId,
            3,
            "Старый коммент",
            _dateTimeProvider);

        var reviewId = targetUser.Reviews.First().Id;
        var oldUpdatedAt = targetUser.Reviews.First().UpdatedAt;
        var wrongAuthorId = Guid.NewGuid();

        // Act
        var updatedReviewResult = targetUser.UpdateReview(
            reviewId,
            wrongAuthorId,
            4,
            "Новый коммент",
            _dateTimeProvider);

        // Assert
        updatedReviewResult.IsError.Should().BeTrue();
        updatedReviewResult.FirstError.Should().Be(UserErrors.NotTheReviewAuthor);

        var updatedReview = targetUser.Reviews.First();

        updatedReview.Rating.Value.Should().Be(3);
        updatedReview.Comment?.Value.Should().Be("Старый коммент");
        updatedReview.UpdatedAt.Should().Be(oldUpdatedAt);
    }

    [Fact]
    public void CreateReview_WhenUserHasNoProfile_ShouldFail()
    {
        // Arrange
        var targetUser = UserFactory.CreateTestUser();
        var authorId = Guid.NewGuid();

        // Act
        var createdReview = targetUser.CreateReview(
            authorId,
            Constants.Review.ValidRating,
            Constants.Review.Comment,
            _dateTimeProvider
        );

        // Assert
        createdReview.IsError.Should().BeTrue();
        createdReview.FirstError.Should().Be(UserErrors.UserMustCreateAUserProfile);
    }

    [Fact]
    public void CreateReview_WhenAuthorIsTarget_ShouldFail()
    {
        // Arrange
        var targetUser = CreateUserWithProfile();
        var authorId = targetUser.Id;

        // Act
        var createdReview = targetUser.CreateReview(
            authorId,
            Constants.Review.ValidRating,
            Constants.Review.Comment,
            _dateTimeProvider
        );

        // Assert
        createdReview.IsError.Should().BeTrue();
        createdReview.FirstError.Should().Be(UserErrors.UserCannotWriteAReviewForThemselves);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(6)]
    public void CreateReview_WhenRatingIsInvalid_ShouldFail(int invalidRating)
    {
        // Arrange
        var targetUser = CreateUserWithProfile();
        var authorId = Guid.NewGuid();

        // Act
        var createdReview = targetUser.CreateReview(
            authorId,
            invalidRating,
            Constants.Review.Comment,
            _dateTimeProvider
        );

        // Assert
        createdReview.IsError.Should().BeTrue();
        createdReview.FirstError.Code.Should().Be("Rating.InvalidRange");
    }

    #endregion

    #region Activation Tests

    [Fact]
    public void DeactivateUser_WhenUserIsActive_ShouldSuccess()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();

        // Act
        var deactivatedUserResult = user.DeactivateUser();

        // Assert
        deactivatedUserResult.IsError.Should().BeFalse();
        user.IsActive.Should().BeFalse();
        user.CanSell().Should().BeFalse();
    }

    [Fact]
    public void DeactivateUser_WhenAlreadyDeactivated_ShouldFail()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();
        user.DeactivateUser();

        // Act
        var deactivatedUserResult = user.DeactivateUser();

        // Assert
        deactivatedUserResult.IsError.Should().BeTrue();
        deactivatedUserResult.FirstError.Should().Be(UserErrors.UserAlreadyDeactivated);
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void ActivateUser_WhenUserIsDeactivated_ShouldSuccess()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();
        user.DeactivateUser();

        // Act
        var activatedUserResult = user.ActivateUser();

        // Assert
        activatedUserResult.IsError.Should().BeFalse();
        user.IsActive.Should().BeTrue();
    }
    [Fact]
    public void ActivateUser_WhenUserIsActivated_ShouldSuccess()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();

        // Act
        var activatedUserResult = user.ActivateUser();

        // Assert
        activatedUserResult.IsError.Should().BeTrue();
        activatedUserResult.FirstError.Should().Be(UserErrors.UserAlreadyActivated);
        user.IsActive.Should().BeTrue();
    }

    #endregion

    #region Helpers

    private static User CreateUserWithProfile()
    {
        var user = UserFactory.CreateTestUser();
        var p = UserProfileFactory.CreateValidParams();

        user.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        return user;
    }

    #endregion
}