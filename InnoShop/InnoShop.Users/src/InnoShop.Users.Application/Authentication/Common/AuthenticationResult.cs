using InnoShop.Users.Domain.UserAggregate;

namespace InnoShop.Users.Application.Authentication.Common;

public record AuthenticationResult(
    User User,
    string Token);
