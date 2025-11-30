using InnoShop.UserManagement.Application.Authentication.Commands.Register;
using InnoShop.UserManagement.Application.Users.Commands.ActivateUserProfile;
using InnoShop.UserManagement.Application.Users.Commands.DeactivateUserProfile;
using InnoShop.UserManagement.Application.Users.Commands.UpdateUserProfile;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.TestCommon.TestConstants;

namespace InnoShop.UserManagement.TestCommon.UserAggregate;

public static class UserProfileCommandFactory
{
    public static RegisterCommand CreateRegisterCommand(
        string? email = null,
        string? password = null)
    {
        return new RegisterCommand(
            email ?? Constants.User.Email.Value,
            password ?? Constants.User.PasswordHash);
    }

    public static UpdateUserProfileCommand CreateUpdateUserProfileCommand(
        Guid? userId = null,
        string? firstName = null,
        string? lastName = null,
        string? avatarUrl = null,
        string? phoneNumber = null,
        Country? country = null,
        string? state = null,
        string? city = null)
    {
        return new UpdateUserProfileCommand(
            userId ?? Guid.NewGuid(),
            firstName ?? Constants.UserProfile.FirstName.Value,
            lastName ?? Constants.UserProfile.LastName.Value,
            avatarUrl ?? Constants.UserProfile.AvatarUrl.Value,
            phoneNumber ?? Constants.UserProfile.ValidPhoneNumberBelarus.Value,
            country ?? Constants.UserProfile.ValidLocationBelarus.Country,
            state ?? Constants.UserProfile.ValidLocationBelarus.State,
            city ?? Constants.UserProfile.ValidLocationBelarus.City);
    }

    public static DeactivateUserProfileCommand CreateDeactivateUserProfileCommand(
        Guid? userId = null)
    {
        return new DeactivateUserProfileCommand(
            userId ?? Guid.NewGuid());
    }

    public static ActivateUserProfileCommand CreateActivateUserProfileCommand(
        Guid? userId = null)
    {
        return new ActivateUserProfileCommand(
            userId ?? Guid.NewGuid());
    }
}