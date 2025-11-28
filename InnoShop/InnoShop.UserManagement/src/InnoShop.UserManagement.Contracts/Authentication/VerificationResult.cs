using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Contracts.Authentication;

public record VerificationResult(
    User User,
    string Token);