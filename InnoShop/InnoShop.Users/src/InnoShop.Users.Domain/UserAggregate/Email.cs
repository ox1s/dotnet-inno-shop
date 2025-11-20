using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public record Email
{
    private Email(string value) => Value = value;

    public string Value { get; }

    public static ErrorOr<Email> Create(string? email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return EmailErrors.Empty;
        }

        if (email.Split('@').Length != 2)
        {
            return EmailErrors.InvalidFormat;
        }

        return new Email(email);
    }
}
