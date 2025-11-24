using ErrorOr;
using FluentAssertions;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.TestCommon.TestConstants;
using InnoShop.UserManagement.TestCommon.TestUtils.Services;
using InnoShop.UserManagement.TestCommon.UserAggregate;

namespace InnoShop.UserManagement.Domain.UnitTests;

public class UserTests
{

    #region UserProfile Tests
    [Fact]
    public void CreateUserProfile_WhenValidData_ShouldCreateProfile()
    {
        // Arrange
        var user = User.CreateUser(
            Constants.User.Email,
            Constants.User.PasswordHash
        );

        // Act
        var userProfile = CreateValidUserProfile();
        var createUserResult = user.CreateUserProfile(userProfile);

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
        var newProfile = CreateValidUserProfile();

        // Act
        var addUserProfile2Result = user.CreateUserProfile(newProfile);

        // Assert
        addUserProfile2Result.IsError.Should().BeTrue();
        addUserProfile2Result.FirstError.Should().Be(UserErrors.CannotCreateMoreThanOneProfile);
    }

    [Fact]
    public void CreateUserProfile_WhenLocationIsNotBelarus_ShouldFail()
    {
        // Arrange
        // Act
        var userProfileResult = UserProfile.Create(
            Constants.UserProfile.FirstName,
            Constants.UserProfile.LastName,
            Constants.UserProfile.AvatarUrl,
            Constants.UserProfile.ValidPhoneNumberUsa,
            Constants.UserProfile.ValidLocationUsa
        );

        // Assert
        userProfileResult.IsError.Should().BeTrue();
        userProfileResult.FirstError.Should().Be(UserErrors.UserProfileMustBeInAllowedCountry);
    }

    [Fact]
    public void CreateUserProfile_WhenPhoneNumberIsInvalid_ShouldFail()
    {
        // Arrange
        // Act
        var invalidPhoneResult = PhoneNumber.Create("123", Country.Belarus);

        // Assert
        invalidPhoneResult.IsError.Should().BeTrue();
        invalidPhoneResult.FirstError.Should().Be(PhoneNumberErrors.Invalid);
    }

    [Fact]
    public void UpdateUserProfile_WhenProfileDoesNotExist_ShouldFail()
    {
        // Arrange
        var user = User.CreateUser(Constants.User.Email, Constants.User.PasswordHash);
        var newProfile = CreateValidUserProfile();

        // Act
        var updateUserProfileResult = user.UpdateUserProfile(newProfile);

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

        // Act
        var invalidProfileResult = UserProfile.Create(
            Constants.UserProfile.FirstName,
            Constants.UserProfile.LastName,
            Constants.UserProfile.AvatarUrl,
            Constants.UserProfile.ValidPhoneNumberUsa,
            Constants.UserProfile.ValidLocationUsa
        );

        // Assert
        invalidProfileResult.IsError.Should().BeTrue();
        invalidProfileResult.FirstError.Should().Be(UserErrors.UserProfileMustBeInAllowedCountry);
        user.UserProfile.Should().NotBeNull();
        user.UserProfile!.Location.Country.Should().Be(Country.Belarus);
    }
    [Fact]
    public void UpdateUserProfile_WhenNewPhoneIsNotFromBelarus_ShouldFail()
    {
        // Arrange
        var user = CreateUserWithProfile();

        // Act
        var invalidPhoneResult = PhoneNumber.Create("+12125550199", Country.Belarus);

        // Assert
        invalidPhoneResult.IsError.Should().BeTrue();
        invalidPhoneResult.FirstError.Should().Be(PhoneNumberErrors.WrongCountry);
        user.UserProfile.Should().NotBeNull();
        user.UserProfile!.Location.Country.Should().Be(Country.Belarus);
    }

    #endregion


    #region Activation Tests

    [Fact]
    public void DeactivateUser_WhenUserIsActive_ShouldSuccess()
    {
        // Arrange
        var user = User.CreateUser(Constants.User.Email, Constants.User.PasswordHash);

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
        var user = User.CreateUser(Constants.User.Email, Constants.User.PasswordHash);
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
        var user = User.CreateUser(Constants.User.Email, Constants.User.PasswordHash);
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
        var user = User.CreateUser(Constants.User.Email, Constants.User.PasswordHash);

        // Act
        var activatedUserResult = user.ActivateUser();

        // Assert
        activatedUserResult.IsError.Should().BeTrue();
        activatedUserResult.FirstError.Should().Be(UserErrors.UserAlreadyActivated);
        user.IsActive.Should().BeTrue();
    }

    #endregion

    #region Helpers

    private static UserProfile CreateValidUserProfile()
    {
        return UserProfile.Create(
            Constants.UserProfile.FirstName,
            Constants.UserProfile.LastName,
            Constants.UserProfile.AvatarUrl,
            Constants.UserProfile.ValidPhoneNumberBelarus,
            Constants.UserProfile.ValidLocationBelarus
        ).Value;
    }

    private static User CreateUserWithProfile()
    {
        var user = User.CreateUser(Constants.User.Email, Constants.User.PasswordHash);
        var profile = CreateValidUserProfile();

        user.CreateUserProfile(profile);

        return user;
    }

    #endregion
}