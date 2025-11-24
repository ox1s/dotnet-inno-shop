using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Users.Authentication.Common;

public record AuthenticationResult(
    User User,
    string Token);
