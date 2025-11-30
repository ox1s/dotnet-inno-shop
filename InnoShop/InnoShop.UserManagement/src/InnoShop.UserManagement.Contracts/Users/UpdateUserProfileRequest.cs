namespace InnoShop.UserManagement.Contracts.Users;

public record UpdateUserProfileRequest(
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Country,
    string? State,
    string? City);