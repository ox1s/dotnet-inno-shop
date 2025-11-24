using InnoShop.UserManagement.Application.Users.Authentication.Commands.Register;
using InnoShop.UserManagement.TestCommon.TestConstants;


namespace InnoShop.UserManagement.TestCommon.UserAggregate;

public static class UserCommandFactory
{
    public static RegisterCommand CreateRegisterCommand(
            string email = "test@test.com",
            string password = "1321")
    {
        return new RegisterCommand(
            Email: email,
            Password: password);
    }
}
