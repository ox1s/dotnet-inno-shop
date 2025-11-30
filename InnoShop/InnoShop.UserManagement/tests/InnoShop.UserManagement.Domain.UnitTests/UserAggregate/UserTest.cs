using ErrorOr;
using FluentAssertions;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.TestCommon.TestConstants;
using InnoShop.UserManagement.TestCommon.UserAggregate;

namespace InnoShop.UserManagement.Domain.UnitTests;

public class UserTests
{

    [Fact]
    public void CreateUserProfile_WhenValidData_ShouldCreateProfile()
    {
        // Arrange
        var user = UserFactory.CreateUser();

        // Act
        var userProfile = UserFactory.CreateUserProfile();
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
        var user = UserFactory.CreateUserWithProfile();
        var newProfile = UserFactory.CreateUserProfile();

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
        var user = UserFactory.CreateUser();
        var newProfile = UserFactory.CreateUserProfile();

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
        var user = UserFactory.CreateUserWithProfile();

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
        var user = UserFactory.CreateUserWithProfile();

        // Act
        var invalidPhoneResult = PhoneNumber.Create("+12125550199", Country.Belarus);

        // Assert
        invalidPhoneResult.IsError.Should().BeTrue();
        invalidPhoneResult.FirstError.Should().Be(PhoneNumberErrors.WrongCountry);
        user.UserProfile.Should().NotBeNull();
        user.UserProfile!.Location.Country.Should().Be(Country.Belarus);
    }


    [Fact]
    public void DeactivateUser_WhenUserIsActive_ShouldSuccess()
    {
        // Arrange
        var user = UserFactory.CreateUser();

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
        var user = UserFactory.CreateUser();
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
        var user = UserFactory.CreateUser();
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
        var user = UserFactory.CreateUser();

        // Act
        var activatedUserResult = user.ActivateUser();

        // Assert
        activatedUserResult.IsError.Should().BeTrue();
        activatedUserResult.FirstError.Should().Be(UserErrors.UserAlreadyActivated);
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RequestPasswordReset_ShouldGenerateTokenAndSetExpiration()
    {
        // Arrange
        var user = UserFactory.CreateUser();

        // Act
        user.RequestPasswordReset();

        // Assert
        user.PasswordResetToken.Should().NotBeNull();
        user.PasswordResetToken.Should().NotBeEmpty();
        user.PasswordResetTokenExpiration.Should().NotBeNull();
        user.PasswordResetTokenExpiration.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void RequestPasswordReset_ShouldGenerateUrlSafeToken()
    {
        // Arrange
        var user = UserFactory.CreateUser();

        // Act
        user.RequestPasswordReset();

        // Assert
        user.PasswordResetToken.Should().NotContain("+");
        user.PasswordResetToken.Should().NotContain("/");
        user.PasswordResetToken.Should().NotContain("=");
    }

    [Fact]
    public void ResetPassword_WhenValidToken_ShouldResetPassword()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user.RequestPasswordReset();
        var token = user.PasswordResetToken!;
        var newPassword = "NewPassword123!";
        var passwordHasher = new TestPasswordHasher();

        // Act
        var result = user.ResetPassword(token, newPassword, passwordHasher);

        // Assert
        result.IsError.Should().BeFalse();
        user.PasswordResetToken.Should().BeNull();
        user.PasswordResetTokenExpiration.Should().BeNull();
        user.IsCorrectPasswordHash(newPassword, passwordHasher).Should().BeTrue();
    }

    [Fact]
    public void ResetPassword_WhenInvalidToken_ShouldReturnError()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user.RequestPasswordReset();
        var invalidToken = "invalid-token";
        var newPassword = "NewPassword123!";
        var passwordHasher = new TestPasswordHasher();

        // Act
        var result = user.ResetPassword(invalidToken, newPassword, passwordHasher);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("User.InvalidResetToken");
        user.PasswordResetToken.Should().NotBeNull(); // Token should still exist
    }


    [Fact]
    public void ResetPassword_WhenTokenIsNull_ShouldReturnError()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        // Don't request password reset, so token is null
        var newPassword = "NewPassword123!";
        var passwordHasher = new TestPasswordHasher();

        // Act
        var result = user.ResetPassword("some-token", newPassword, passwordHasher);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("User.InvalidResetToken");
    }

    private class TestPasswordHasher : IPasswordHasher
    {
        public ErrorOr<string> HashPassword(string password)
        {
            return $"hashed_{password}";
        }

        public bool IsCorrectPassword(string password, string hash)
        {
            return hash == $"hashed_{password}";
        }
    }
}