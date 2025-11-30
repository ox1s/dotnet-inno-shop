using System.Text.RegularExpressions;
using Ardalis.SmartEnum;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public class Country : SmartEnum<Country>
{
    public static readonly Country Belarus = new(
        nameof(Belarus),
        0,
        "Беларусь",
        "BY",
        "+375",
        @"^(?:\+?375|80)(?:25|29|33|44)\d{7}$");

    public static readonly Country Usa = new(
        nameof(Usa),
        1,
        "США",
        "US",
        "+1",
        @"^(?:\+1)?[ -]?\(?\d{3}\)?[ -]?\d{3}[ -]?\d{4}$");

    public static readonly IReadOnlyList<Country> AllowedCountries = new List<Country> { Belarus }.AsReadOnly();
    private readonly Regex _cachedRegex;

    protected Country(string name, int value, string displayName, string isoCode, string phoneCode, string regexPattern)
        : base(name, value)
    {
        DisplayName = displayName;
        IsoCode = isoCode;
        PhoneCode = phoneCode;
        RegexPattern = regexPattern;
        _cachedRegex = new Regex(regexPattern, RegexOptions.Compiled);
    }

    public string DisplayName { get; }
    public string PhoneCode { get; }
    public string RegexPattern { get; }
    public string IsoCode { get; }
}