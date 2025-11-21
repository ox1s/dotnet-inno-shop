using InnoShop.Users.Domain.UserAggregate;
using InnoShop.Users.TestCommon.TestConstants;

namespace InnoShop.Users.TestCommon.UserAggregate;

public record UserProfileParams(
    FirstName FirstName,
    LastName LastName,
    AvatarUrl AvatarUrl,
    string PhoneNumber,
    string Country,
    string State,
    string City
);

public static class UserProfileFactory
{
    public static UserProfileParams CreateValidParams()
    {
        return new UserProfileParams(
            Constants.UserProfile.FirstName,
            Constants.UserProfile.LastName,
            Constants.UserProfile.AvatarUrl,
            Constants.UserProfile.ValidPhoneNumberBelarus,
            Constants.UserProfile.BelarusCountryName,
            Constants.UserProfile.BelarusState,
            Constants.UserProfile.BelarusCity
        );
    }
}