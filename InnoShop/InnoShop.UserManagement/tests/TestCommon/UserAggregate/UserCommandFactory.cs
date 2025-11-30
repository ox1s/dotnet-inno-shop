using InnoShop.UserManagement.Application.Authentication.Commands.Register;
using InnoShop.UserManagement.TestCommon.TestConstants;

namespace InnoShop.UserManagement.TestCommon.UserAggregate;

public static class UserCommandFactory
{
    public static RegisterCommand CreateCreateUserCommand(
        string? email = null,
        string? password = null)
    {
        return new RegisterCommand(
            email ?? Constants.User.Email.Value,
            password ?? Constants.User.PasswordHash);
    }
}