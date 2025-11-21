using ErrorOr;
using FluentAssertions;
using InnoShop.Users.Domain.UserAggregate;
using InnoShop.Users.TestCommon.TestConstants;
using InnoShop.Users.TestCommon.TestUtils.Services;
using InnoShop.Users.TestCommon.UserAggregate;

namespace InnoShop.Users.Domain.UnitTests;

public class UserTest
{
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
        var user = UserFactory.CreateTestUser();
        var p = UserProfileFactory.CreateValidParams();

        // Act
        var addUserProfile1Result = user.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        var addUserProfile2Result = user.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        // Assert
        addUserProfile1Result.IsError.Should().BeFalse();

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

        createUserResult.FirstError.Should().Be(UserErrors.UserProfileMustBeInBelarus);
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
        var user = UserFactory.CreateTestUser();
        var p = UserProfileFactory.CreateValidParams();
        user.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        p = p with
        {
            Country = Constants.UserProfile.UsaCountryName
        };
        // Act

        var updateUserProfileResult = user.UpdateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);

        // Assert
        updateUserProfileResult.IsError.Should().BeTrue();
        updateUserProfileResult.FirstError.Should().Be(UserErrors.UserProfileMustBeInBelarus);
        user.UserProfile.Should().NotBeNull();
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
            new TestDateTimeProvider()
        );
        // Assert
        createdReview.IsError.Should().BeTrue();
        createdReview.FirstError.Should().Be(UserErrors.UserMustCreateAUserProfile);
        createdReview.IsError.Should().BeTrue();
    }
    [Fact]
    public void CreateReview_WhenAuthorIsTarget_ShouldFail()
    {
        // Arrange
        var targetUser = UserFactory.CreateTestUser();
        var p = UserProfileFactory.CreateValidParams();
        targetUser.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);
        var authorId = targetUser.Id;

        // Act
        var createdReview = targetUser.CreateReview(
            authorId,
            Constants.Review.ValidRating,
            Constants.Review.Comment,
            new TestDateTimeProvider()
        );
        // Assert
        createdReview.IsError.Should().BeTrue();
        createdReview.FirstError.Should().Be(UserErrors.UserCannotWriteAReviewForThemselves);
        createdReview.IsError.Should().BeTrue();
    }
    [Fact]
    public void CreateReview_WhenRatingIsInvalid_ShouldFail()
    {
        // Arrange
        var targetUser = UserFactory.CreateTestUser();
        var p = UserProfileFactory.CreateValidParams();
        targetUser.CreateUserProfile(
            p.FirstName, p.LastName, p.AvatarUrl, p.PhoneNumber,
            p.Country, p.State, p.City);
        var authorId = Guid.NewGuid();
        var rating = -1;

        // Act
        var createdReview = targetUser.CreateReview(
            authorId,
            rating,
            Constants.Review.Comment,
            new TestDateTimeProvider()
        );
        // Assert
        createdReview.FirstError.Code.Should().Be("Rating.CannotHaveRatingDifferentFromOneToFive");
    }
    [Fact]
    public void DeactivateUser_WhenUserIsActive_ShouldSuccessAndRaiseEvent()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();
        // Act
        var deactivateUserResult = user.DeactivateUser();

        // Assert
        deactivateUserResult.IsError.Should().BeFalse();
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
        var deactivateUserResult = user.DeactivateUser();

        // Assert
        deactivateUserResult.IsError.Should().BeTrue();
        deactivateUserResult.FirstError.Should().Be(UserErrors.UserAlreadyDeactivated);
        user.IsActive.Should().BeFalse();
        user.CanSell().Should().BeFalse();
    }
    [Fact]
    public void ActivateUser_WhenUserIsDeactivated_ShouldSuccess()
    {
        // Arrange
        var user = UserFactory.CreateTestUser();
        var deactivateUserResult = user.DeactivateUser();

        // Act
        var activatedUserResult = user.ActivateUser();

        // Assert
        deactivateUserResult.IsError.Should().BeFalse();
        activatedUserResult.IsError.Should().BeFalse();
        user.IsActive.Should().BeTrue();
    }
}