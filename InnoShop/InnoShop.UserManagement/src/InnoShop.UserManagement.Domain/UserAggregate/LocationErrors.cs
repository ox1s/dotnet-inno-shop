using ErrorOr;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public static class LocationErrors
{
    public static Error InvalidCountry(Country country) => Error.Validation(
        "Location.InvalidCountry",
        $"Country '{country}' is not supported.");
    public static readonly Error InvalidState = Error.Validation(
        "Location.InvalidState",
        "State must be 1-100 characters.");

    public static readonly Error InvalidCity = Error.Validation(
        "Location.InvalidCity",
        "City must be 1-100 characters.");


}
