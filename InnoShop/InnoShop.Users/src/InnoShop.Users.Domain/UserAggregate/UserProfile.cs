
namespace InnoShop.Users.Domain.UserAggregate;

public partial record UserProfile 
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
    private UserProfile() { }
}