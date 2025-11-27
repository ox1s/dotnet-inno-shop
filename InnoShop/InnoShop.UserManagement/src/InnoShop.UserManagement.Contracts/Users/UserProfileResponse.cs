
namespace InnoShop.UserManagement.Contracts.Users;

public record UserProfileResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string AvatarUrl,
    string PhoneNumber,
    string Country,
    string City);


