using ErrorOr;

namespace InnoShop.UserManagement.Application.Users.Authentication.Common;

public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials = Error.Validation(
        code: "Authentication.InvalidCredentials",
        description: "Invalid credentials");
}
