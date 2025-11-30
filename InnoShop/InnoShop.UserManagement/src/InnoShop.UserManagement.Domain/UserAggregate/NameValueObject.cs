using System.Text.RegularExpressions;
using ErrorOr;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public abstract record NameValueObject(string Value)
{
    protected static ErrorOr<string> Validate(string value, string prefix)
    {
        if (string.IsNullOrWhiteSpace(value)) return NameErrors.Empty(prefix);

        if (value.Length < 2 || value.Length > 50) return NameErrors.InvalidLength(prefix);

        if (!Regex.IsMatch(value, @"^[a-zA-Zа-яА-ЯёЁ\- ]+$")) return NameErrors.InvalidChars(prefix);
        return value;
    }
}