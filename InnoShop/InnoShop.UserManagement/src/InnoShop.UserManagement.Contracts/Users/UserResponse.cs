namespace InnoShop.UserManagement.Contracts.Users;

public record UserResponse(
    Guid Id,
    string Email,
    List<string> Roles,
    bool IsEmailVerified,
    bool IsActive,
    UserProfileResponse? Profile
);