
using ErrorOr;

namespace InnoShop.UserManagement.Domain.UserAggregate;

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
        PhoneNumber phoneNumber,
        Location location)
    {

        if (!Country.AllowedCountries.Contains(location.Country))
        {
            return UserErrors.UserProfileMustBeInAllowedCountry;
        }

        return new UserProfile(
            firstName,
            lastName,
            avatarUrl,
            phoneNumber,
            location);
    }

    private UserProfile() { }
}