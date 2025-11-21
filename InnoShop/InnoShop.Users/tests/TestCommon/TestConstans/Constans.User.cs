using InnoShop.Users.Domain.UserAggregate;

namespace InnoShop.Users.TestCommon.TestConstants;

public static partial class Constants
{
    public static class User
    {
        public static readonly Email Email = new Email("test@gmail.com");
        public static readonly string PasswordHash = "$2y$10$f3D3PGNZInAAE6LdocxKmuZ5xQkDCPJYZfc5vBzeWLgNJi/plnFhy";
    }

}
