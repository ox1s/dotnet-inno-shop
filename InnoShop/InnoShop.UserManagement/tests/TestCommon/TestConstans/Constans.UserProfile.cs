using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.TestCommon.TestConstants;

public static partial class Constants
{
    public static class UserProfile
    {
        public static readonly FirstName FirstName = new("First");
        public static readonly LastName LastName = new("Last");
        public static readonly Email Email = new("test@test.com");
        public static readonly AvatarUrl AvatarUrl = new("https://i.pinimg.com/736x/c8/cc/24/c8cc24bba37a25c009647b8875aae0e3.jpg");
        public static readonly PhoneNumber ValidPhoneNumberBelarus = new PhoneNumber("+375291112233");
        public static readonly PhoneNumber ValidPhoneNumberUsa = new PhoneNumber("+12125550199");
        public static readonly Location ValidLocationBelarus = new Location(Country.Belarus, "Витебск", "Витебск");
        public static readonly Location ValidLocationUsa = new Location(Country.Usa, "New Yourk", "New Yourk");

    }
}
