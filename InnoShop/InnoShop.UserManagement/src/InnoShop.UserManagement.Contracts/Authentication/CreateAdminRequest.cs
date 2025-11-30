namespace InnoShop.UserManagement.Contracts.Authentication;

public record CreateAdminRequest(
    string Email,
    string Password);
