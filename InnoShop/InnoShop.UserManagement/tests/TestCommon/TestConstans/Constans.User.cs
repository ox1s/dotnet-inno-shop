using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.TestCommon.TestConstants;

public static partial class Constants
{
    public static class User
    {
        public static readonly Email Email = new("test@test.com");
        public static readonly string PasswordHash = "$2y$10$f3D3PGNZInAAE6LdocxKmuZ5xQkDCPJYZfc5vBzeWLgNJi/plnFhy";

        public static readonly List<string> Permissions =
        [
            AppPermissions.Review.Create,
            AppPermissions.Review.Read,
            AppPermissions.Review.Delete,
            AppPermissions.Review.Update
        ];

        public static readonly List<string> Roles =
        [
            AppRoles.Admin,
            AppRoles.Registered
        ];
    }
}