namespace InnoShop.UserManagement.Contracts.Authentication;

public record VerifyEmailRequest(
    Guid UserId,
    string Token
);