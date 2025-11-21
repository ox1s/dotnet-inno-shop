using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public record Location
{
    private Location(
        Country country,
        string state,
        string? city)
    {
        Country = country;
        State = state;
        City = city;
    }

    public Country Country { get; }
    public string State { get; }
    public string? ZipCode { get; }
    public string? City { get; }
    public string? Street { get; }

    public static ErrorOr<Location> Create(
        string countryName,
        string state,
        string? city)
    {
        if (!Country.TryFromName(countryName, ignoreCase: true, out var parsedCountry))
        {
            return Error.Validation(
                code: "Location.InvalidCountry",
                description: $"Country '{countryName}' is not supported.");
        }
        return new Location(parsedCountry, state, city);
    }

}
