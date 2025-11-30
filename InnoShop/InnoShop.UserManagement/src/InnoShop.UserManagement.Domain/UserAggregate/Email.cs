using System.Net.Mail;
using ErrorOr;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public sealed record Email(string Value)
{
    public static ErrorOr<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return EmailErrors.Empty;
        if (value.Length > 254) return EmailErrors.TooLong;
        if (!MailAddress.TryCreate(value, out _)) return EmailErrors.InvalidFormat;

        return new Email(value);
    }
}