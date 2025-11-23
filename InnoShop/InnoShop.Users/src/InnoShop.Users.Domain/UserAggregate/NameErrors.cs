using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public static class NameErrors
{
    public static Error Empty(string prefix) => Error.Validation(
        $"{prefix}.Empty",
        $"{prefix} cannot be empty.");

    public static Error InvalidLength(string prefix) => Error.Validation(
        $"{prefix}.InvalidLength",
        $"{prefix} must be 1-50 characters.");

    public static Error InvalidChars(string prefix) => Error.Validation(
        $"{prefix}.InvalidChars",
        $"{prefix} can only contain letters, hyphens, or spaces.");
}
