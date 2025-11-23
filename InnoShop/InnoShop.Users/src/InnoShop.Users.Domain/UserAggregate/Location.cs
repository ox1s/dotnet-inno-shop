using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record Location
{
    public Country Country { get; }
    public string State { get; }
    public string? City { get; }

    private Location(Country country, string state, string? city)
    {
        Country = country;
        State = state;
        City = city;
    }

    public static ErrorOr<Location> Create(string countryName, string state, string? city)
    {
        if (!Country.TryFromName(countryName, ignoreCase: true, out var parsedCountry))
            return Error.Validation("Location.InvalidCountry", $"Country '{countryName}' is not supported.");

        if (string.IsNullOrWhiteSpace(state) || state.Length > 100)
            return Error.Validation("Location.InvalidState", "State must be 1-100 characters.");

        if (city is not null && (string.IsNullOrWhiteSpace(city) || city.Length > 100))
            return Error.Validation("Location.InvalidCity", "City must be 1-100 characters if provided.");

        return new Location(parsedCountry, state, city);
    }
}