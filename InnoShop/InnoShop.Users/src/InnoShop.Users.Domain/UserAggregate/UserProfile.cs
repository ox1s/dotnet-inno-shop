
using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public record UserProfile
{
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public AvatarUrl AvatarUrl { get; }
    public PhoneNumber PhoneNumber { get; }
    public Location Location { get; }


    internal UserProfile(
        FirstName firstName,
        LastName lastName,
        AvatarUrl avatarUrl,
        PhoneNumber phoneNumber,
        Location location)
    {
        FirstName = firstName;
        LastName = lastName;
        AvatarUrl = avatarUrl;
        PhoneNumber = phoneNumber;
        Location = location;
    }
    public static ErrorOr<UserProfile> Create(
        FirstName firstName,
        LastName lastName,
        AvatarUrl avatarUrl,
        string rawPhoneNumber,
        string countryName,
        string state,
        string? city)
    {
        var locationResult = Location.Create(countryName, state, city);
        if (locationResult.IsError)
        {
            return locationResult.Errors;
        }
        var location = locationResult.Value;

        if (!Country.AllowedCountries.Contains(location.Country))
        {
            return UserErrors.UserProfileMustBeInAllowedCountry;
        }

        var phoneNumberResult = PhoneNumber.Create(rawPhoneNumber, location.Country);
        if (phoneNumberResult.IsError)
        {
            return phoneNumberResult.Errors;
        }
        var phoneNubmer = phoneNumberResult.Value;

        return new UserProfile(
            firstName,
            lastName,
            avatarUrl,
            phoneNubmer,
            location);
    }

    private UserProfile() { }
}