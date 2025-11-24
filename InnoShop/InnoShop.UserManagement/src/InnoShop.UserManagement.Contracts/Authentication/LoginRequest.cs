namespace InnoShop.UserManagement.Contracts.Authentication;

public record LoginRequest(
    string Email,
    string Password);