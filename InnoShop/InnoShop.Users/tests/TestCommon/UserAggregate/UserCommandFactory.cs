using InnoShop.Users.Application.Authentication.Commands.Register;
using InnoShop.Users.TestCommon.TestConstants;


namespace InnoShop.Users.TestCommon.UserAggregate;

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
