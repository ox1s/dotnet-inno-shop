using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.TestCommon.TestConstants;

namespace InnoShop.UserManagement.TestCommon.UserAggregate;

public static class UserFactory
{
    public static User CreateUser(
        Email? email = null,
        string? passwordHash = null)
    {
        return User.CreateUser(
            email ?? Constants.User.Email,
            passwordHash ?? Constants.User.PasswordHash
        );
    }

    public static UserProfile CreateUserProfile(
        FirstName? firstName = null,
        LastName? lastName = null,
        AvatarUrl? avatarUrl = null,
        PhoneNumber? phoneNumber = null,
        Location? location = null)
    {
        return UserProfile.Create(
            firstName ?? Constants.UserProfile.FirstName,
            lastName ?? Constants.UserProfile.LastName,
            avatarUrl ?? Constants.UserProfile.AvatarUrl,
            phoneNumber ?? Constants.UserProfile.ValidPhoneNumberBelarus,
            location ?? Constants.UserProfile.ValidLocationBelarus
        ).Value;
    }

    public static User CreateUserWithProfile(
        Email? email = null,
        UserProfile? userProfile = null)
    {
        var user = CreateUser(email);
        var profile = userProfile ?? CreateUserProfile();

        user.CreateUserProfile(profile);

        return user;
    }
}