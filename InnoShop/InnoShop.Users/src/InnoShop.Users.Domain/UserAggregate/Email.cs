using ErrorOr;
using System.Net.Mail;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record Email
{
    public string Value { get; }
    public string OriginalValue { get; }

    private Email(string originalValue, string normalizedValue)
    {
        OriginalValue = originalValue;
        Value = normalizedValue;
    }

    public static ErrorOr<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Email.Empty", "Email cannot be empty.");

        if (value.Length > 254)
            return Error.Validation("Email.TooLong", "Email must be under 254 characters.");

        if (!MailAddress.TryCreate(value, out _))
            return Error.Validation("Email.InvalidFormat", "Invalid email format.");

        var normalized = value.ToLowerInvariant();
        return new Email(value, normalized);
    }
}