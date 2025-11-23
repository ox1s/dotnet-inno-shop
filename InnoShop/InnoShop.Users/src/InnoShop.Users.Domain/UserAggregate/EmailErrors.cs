using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public static class EmailErrors
{
    public static readonly Error Empty = Error.Validation(
        "Email.Empty",
        "Email is empty");

    public static readonly Error InvalidFormat = Error.Validation(
        "Email.InvalidFormat",
        "Email format is invalid");
    public static readonly Error TooLong = Error.Validation(
       "Email.InvalidFormat",
       "Email must be under 254 characters.");
}
