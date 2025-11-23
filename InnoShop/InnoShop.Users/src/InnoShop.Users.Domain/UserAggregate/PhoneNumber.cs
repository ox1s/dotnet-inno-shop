using ErrorOr;
using PhoneNumbers;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static ErrorOr<PhoneNumber> Create(string rawNumber, Country country)
    {
        var phoneUtil = PhoneNumberUtil.GetInstance();
        try
        {
            var parsed = phoneUtil.Parse(rawNumber, country.IsoCode);
            if (!phoneUtil.IsValidNumber(parsed))
                return Error.Validation("PhoneNumber.Invalid", "Invalid phone number for country.");

            var actualRegion = phoneUtil.GetRegionCodeForNumber(parsed);
            if (actualRegion != country.IsoCode)
                return Error.Validation("PhoneNumber.WrongCountry", "Phone number does not match the specified country.");

            var normalized = phoneUtil.Format(parsed, PhoneNumberFormat.E164);
            return new PhoneNumber(normalized);
        }
        catch (NumberParseException)
        {
            return Error.Validation("PhoneNumber.ParseError", "Could not parse phone number.");
        }
    }

    public string FormattedValue(Country country) => $"{country.PhoneCode}{Value}";
}