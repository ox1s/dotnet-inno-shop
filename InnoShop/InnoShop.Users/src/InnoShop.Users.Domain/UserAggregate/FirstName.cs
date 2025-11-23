using ErrorOr;
using System.Text.RegularExpressions;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record FirstName
{
    public string Value { get; }

    private FirstName(string value) => Value = value;

    public static ErrorOr<FirstName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("FirstName.Empty", "First name cannot be empty.");

        if (value.Length < 1 || value.Length > 50)
            return Error.Validation("FirstName.InvalidLength", "First name must be 1-50 characters.");

        if (!Regex.IsMatch(value, @"^[a-zA-Zа-яА-ЯёЁ\- ]+$"))
            return Error.Validation("FirstName.InvalidChars", "First name can only contain letters, hyphens, or spaces.");

        return new FirstName(value);
    }
}