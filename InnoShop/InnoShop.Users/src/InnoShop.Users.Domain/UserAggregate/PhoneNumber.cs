using System.Text.RegularExpressions;
using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public record PhoneNumber
{
    public const string Motif = @"^(\+?375|80)?(29|33|44|25)\d{7}$";

    public static Error CannotBePhoneNumberFromOtherCountry => Error.Validation(
            "PhoneNumber.CannotBePhoneNumberFromOtherCountry",
            "The phone number must be in a valid Belarusian format (+375XXYYYYYYY or 80XXYYYYYYY).");
    public string Value { get; init; }
    private PhoneNumber(string value) => Value = value;

    public static ErrorOr<PhoneNumber> Create(string number)
    {
        string normalizedNumber = Regex.Replace(number, @"[^\d\+]", "");

        if (!Regex.IsMatch(normalizedNumber, Motif))
        {
            return CannotBePhoneNumberFromOtherCountry;
        }

        if (normalizedNumber.StartsWith("80"))
        {
            normalizedNumber = "+375" + normalizedNumber.Substring(2);
        }
        else if (normalizedNumber.StartsWith("375"))
        {
            normalizedNumber = "+" + normalizedNumber;
        }

        return new PhoneNumber(normalizedNumber);
    }
}