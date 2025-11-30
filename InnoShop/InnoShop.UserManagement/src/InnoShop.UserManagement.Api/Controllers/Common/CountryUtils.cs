using ErrorOr;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Api.Controllers.Common;

public static class CountryUtils
{
    public static ErrorOr<Country> ToDomain(string countryName)
    {
        if (string.IsNullOrWhiteSpace(countryName))
            return Error.Validation("Country.Empty", "Country cannot be empty.");

        if (Country.TryFromName(countryName, true, out var country)) return country;

        return Error.Validation("Country.Invalid", $"Invalid country '{countryName}'");
    }
}