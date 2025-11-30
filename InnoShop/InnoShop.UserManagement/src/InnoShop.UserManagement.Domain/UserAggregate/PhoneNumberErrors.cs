using ErrorOr;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public static class PhoneNumberErrors
{
    public static readonly Error Invalid = Error.Validation(
        "PhoneNumber.Invalid",
        "Invalid phone number for country.");

    public static readonly Error WrongCountry = Error.Validation(
        "PhoneNumber.WrongCountry",
        "Phone number does not match the specified country.");

    public static readonly Error ParseError = Error.Validation(
        "PhoneNumber.ParseError",
        "Could not parse phone number.");
}