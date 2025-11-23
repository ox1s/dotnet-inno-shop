using ErrorOr;
using System;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record AvatarUrl(string Value)
{
    public static ErrorOr<AvatarUrl> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return AvatarUrlErrors.Empty;
        }
        if (value.Length > 2048)
        {
            return AvatarUrlErrors.TooLong;
        }
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || (uri.Scheme != "http" && uri.Scheme != "https"))

        {
            return AvatarUrlErrors.Invalid;
        }

        return new AvatarUrl(value);
    }

}