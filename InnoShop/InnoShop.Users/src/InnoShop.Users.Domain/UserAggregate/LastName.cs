using ErrorOr;
using System.Text.RegularExpressions;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record LastName
{
    public string Value { get; }

    private LastName(string value) => Value = value;

    public static ErrorOr<LastName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("LastName.Empty", "Last name cannot be empty.");

        if (value.Length < 1 || value.Length > 50)
            return Error.Validation("LastName.InvalidLength", "Last name must be 1-50 characters.");

        if (!Regex.IsMatch(value, @"^[a-zA-Zа-яА-ЯёЁ\- ]+$"))
            return Error.Validation("LastName.InvalidChars", "Last name can only contain letters, hyphens, or spaces.");

        return new LastName(value);
    }
}