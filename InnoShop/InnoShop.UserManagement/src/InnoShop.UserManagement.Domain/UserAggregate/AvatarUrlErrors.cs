using ErrorOr;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public static class AvatarUrlErrors
{
    public static readonly Error Empty = Error.Validation(
        "AvatarUrl.Empty",
        "Avatar URL cannot be empty.");

    public static readonly Error TooLong = Error.Validation(
        "AvatarUrl.TooLong",
        "Avatar URL must be under 2048 characters.");

    public static readonly Error Invalid = Error.Validation(
        "AvatarUrl.Invalid",
        "Must be a valid absolute HTTP/HTTPS URL.");
}