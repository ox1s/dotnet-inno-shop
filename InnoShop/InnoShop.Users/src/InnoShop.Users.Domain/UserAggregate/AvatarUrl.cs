using ErrorOr;
using System;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record AvatarUrl
{
    public string Value { get; }

    private AvatarUrl(string value) => Value = value;

    public static ErrorOr<AvatarUrl> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("AvatarUrl.Empty", "Avatar URL cannot be empty.");

        if (value.Length > 2048)
            return Error.Validation("AvatarUrl.TooLong", "Avatar URL must be under 2048 characters.");

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != "http" && uri.Scheme != "https"))
            return Error.Validation("AvatarUrl.Invalid", "Must be a valid absolute HTTP/HTTPS URL.");

        return new AvatarUrl(value);
    }

}