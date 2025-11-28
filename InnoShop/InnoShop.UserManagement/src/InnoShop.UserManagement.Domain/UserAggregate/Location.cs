using ErrorOr;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public sealed record Location(Country Country, string State, string City)
{
    public static ErrorOr<Location> Create(Country country, string state, string city)
    {
        if (!Country.List.Contains(country))
        {
            return LocationErrors.InvalidCountry(country);

        }
        if (string.IsNullOrWhiteSpace(state) || state.Length > 100)
        {
            return LocationErrors.InvalidState;
        }
        if (string.IsNullOrWhiteSpace(city) || city.Length > 100)
        {
            return LocationErrors.InvalidCity;
        }
        return new Location(country, state, city);
    }
}