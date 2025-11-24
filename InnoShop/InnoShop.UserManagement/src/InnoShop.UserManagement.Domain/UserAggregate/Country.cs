using System.Text.RegularExpressions;
using Ardalis.SmartEnum;
using ErrorOr;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public class Country : SmartEnum<Country>
{
    public string DisplayName { get; }
    public string PhoneCode { get; }
    public string RegexPattern { get; }
    public string IsoCode { get; }
    private readonly Regex _cachedRegex;


    public static readonly Country Belarus = new(
        name: nameof(Belarus),
        value: 0,
        displayName: "Беларусь",
        isoCode: "BY",
        phoneCode: "+375",
        regexPattern: @"^(?:\+?375|80)(?:25|29|33|44)\d{7}$");

    public static readonly Country USA = new(
        name: nameof(USA),
        value: 1,
        displayName: "США",
        isoCode: "US",
        phoneCode: "+1",
        regexPattern: @"^(?:\+1)?[ -]?\(?\d{3}\)?[ -]?\d{3}[ -]?\d{4}$");

    public static readonly IReadOnlyList<Country> AllowedCountries = new List<Country> { Belarus }.AsReadOnly();

    protected Country(string name, int value, string displayName, string isoCode, string phoneCode, string regexPattern)
        : base(name, value)
    {
        DisplayName = displayName;
        IsoCode = isoCode;
        PhoneCode = phoneCode;
        RegexPattern = regexPattern;
        _cachedRegex = new Regex(regexPattern, RegexOptions.Compiled);
    }

}