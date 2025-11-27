using InnoShop.UserManagement.Application.Authentication.Commands.Register;
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
            Email: email ?? Constants.User.Email.Value,
            Password: password ?? Constants.User.PasswordHash);
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
            UserId: userId ?? Guid.NewGuid(),
            FirstName: firstName ?? Constants.UserProfile.FirstName.Value,
            LastName: lastName ?? Constants.UserProfile.LastName.Value,
            AvatarUrl: avatarUrl ?? Constants.UserProfile.AvatarUrl.Value,
            PhoneNumber: phoneNumber ?? Constants.UserProfile.ValidPhoneNumberBelarus.Value,
            Country: country ?? Constants.UserProfile.ValidLocationBelarus.Country,
            State: state ?? Constants.UserProfile.ValidLocationBelarus.State,
            City: city ?? Constants.UserProfile.ValidLocationBelarus.City);
    }
}
