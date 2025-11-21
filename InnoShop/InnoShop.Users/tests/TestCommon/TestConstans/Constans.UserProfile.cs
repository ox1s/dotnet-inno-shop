using InnoShop.Users.Domain.UserAggregate;

namespace InnoShop.Users.TestCommon.TestConstants;

public static partial class Constants
{
    public static class UserProfile
    {
        public static readonly FirstName FirstName = new FirstName("Гоголь");
        public static readonly LastName LastName = new LastName("Моголь");
        public static readonly AvatarUrl AvatarUrl = new AvatarUrl("https/images");
        public static readonly string ValidPhoneNumberBelarus = "+375291112233";
        public static readonly string ValidPhoneNumberUsa = "+12125550199";


        public static readonly string BelarusCountryName = "Belarus";
        public static readonly string BelarusState = "Витебская область";
        public static readonly string BelarusCity = "Витебск";

        public static readonly string UsaCountryName = "USA";
        public static readonly string UsaState = "New York";
        public static readonly string UsaCity = "New York";
    }
}
