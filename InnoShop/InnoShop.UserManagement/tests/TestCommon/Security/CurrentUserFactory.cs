using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.TestCommon.TestConstants;

namespace TestCommon.Security;

public static class CurrentUserFactory
{
    public static CurrentUser CreateCurrentUser(
        Guid? id = null,
        string? email = null,
        IReadOnlyList<string>? permissions = null,
        IReadOnlyList<string>? roles = null)
    {
        return new CurrentUser(
            id ?? Guid.NewGuid(),
            email ?? Constants.User.Email.Value,
            permissions ?? Constants.User.Permissions,
            roles ?? Constants.User.Roles);
    }
}