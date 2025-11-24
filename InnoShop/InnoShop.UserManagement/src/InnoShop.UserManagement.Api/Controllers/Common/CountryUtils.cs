using ErrorOr;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Api.Controllers.Common;

public static class CountryUtils
{
    public static ErrorOr<List<Country>> ToDomain(List<string>? countries)
    {
        if (countries is null)
        {
            return new List<Country>();
        }

        List<Country> parsedCountries = countries
            .Select(country => Country.TryFromName(country, out var parsedCategory) ? parsedCategory : null)
            .Where(country => country is not null)
            .ToList()!;

        if (parsedCountries.Count != countries.Count)
        {
            return countries.Except(parsedCountries.ConvertAll(country => country.Name))
                .Select(invalidCountry => Error.Validation("Countries.InvalidCategory", $"Invalid country '{invalidCountry}'"))
                .ToList();
        }

        return parsedCountries;
    }
}
