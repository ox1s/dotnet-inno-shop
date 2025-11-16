namespace UserManagement.Domain.UnitTests.TestUtils.Users;

public static class UserFactory
{
    public static User CreateUser(Guid? id = null)
    {
        return new User(
            id: id ?? Constants.User.Id);
    }
}