using InnoShop.Users.Domain.UserAggregate;
using InnoShop.Users.TestCommon.TestConstants;


namespace InnoShop.Users.TestCommon.UserAggregate;

public static class UserFactory
{
    public static User CreateTestUser(
        Email? email = null,
        string? passwordHash = null)
    {
        User user = User.CreateUser(
            email: email ?? Constants.User.Email,
            passwordHash: passwordHash ?? Constants.User.PasswordHash
        );

        return user;
    }
}
