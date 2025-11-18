using ErrorOr;

namespace InnoShop.Users.Domain.Users;

public static class EmailErrors
{
    public static readonly Error Empty = Error.Validation(
        code: "Email.Empty",
        description: "Email cannot be empty.");

    public static readonly Error InvalidFormat = Error.Validation(
        code: "Email.InvalidFormat",
        description: "Email format is invalid.");
}
