using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public record PhoneNumber
{
    public string Value { get; init; }
    internal PhoneNumber(string value) => Value = value;

    public static ErrorOr<PhoneNumber> Create(string number, Country country)
    {
        if (!country.IsValidPhoneNumber(number))
        {
            return Error.Validation(
                "PhoneNumber.InvalidForCountry",
                $"Phone number is not valid for {country.Name}."
            );
        }

        return new PhoneNumber(number);
    }
}