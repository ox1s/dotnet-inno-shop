namespace InnoShop.UserManagement.Contracts.Authentication;

public record RegisterRequest(
    string Email,
    string Password);