using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.TestCommon.TestConstants;

public static partial class Constants
{
    public static class User
    {
        public static readonly Email Email = new("test@test.com");
        public static readonly string PasswordHash = "$2y$10$f3D3PGNZInAAE6LdocxKmuZ5xQkDCPJYZfc5vBzeWLgNJi/plnFhy";
    }

}
