using ErrorOr;
using PhoneNumbers;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public sealed record PhoneNumber(string Value)
{
    public static ErrorOr<PhoneNumber> Create(string rawNumber, Country country)
    {
        var phoneUtil = PhoneNumberUtil.GetInstance();
        try
        {
            var parsed = phoneUtil.Parse(rawNumber, country.IsoCode);
            if (!phoneUtil.IsValidNumber(parsed)) return PhoneNumberErrors.Invalid;
            var actualRegion = phoneUtil.GetRegionCodeForNumber(parsed);
            if (actualRegion != country.IsoCode) return PhoneNumberErrors.WrongCountry;
            var normalized = phoneUtil.Format(parsed, PhoneNumberFormat.E164);
            return new PhoneNumber(normalized);
        }
        catch (NumberParseException)
        {
            return PhoneNumberErrors.ParseError;
        }
    }

    public string FormattedValue(Country country)
    {
        return $"{country.PhoneCode}{Value}";
    }
}