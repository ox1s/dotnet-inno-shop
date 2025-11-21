using System.Text.RegularExpressions;
using Ardalis.SmartEnum;

namespace InnoShop.Users.Domain.UserAggregate;

public class Country : SmartEnum<Country>
{
    public string DisplayName { get; }
    public string PhoneCode { get; }
    public string RegexPattern { get; }

    /// <summary>
    /// Нормализованный номер должен удовлетворять паттерну:
    /// +375259876543
    /// 80259876543
    /// +375(29)....
    /// нормализация вне домена
    /// </summary>
    public static readonly Country Belarus = new(
       name: nameof(Belarus),
       value: 0,
       displayName: "Беларусь",
       phoneCode: "+375",
       regexPattern: @"^(?:\+?375|80)(?:25|29|33|44)\d{7}$"
    );

    public static readonly Country USA = new(
       name: nameof(USA),
       value: 1,
       displayName: "США",
       phoneCode: "+1",
       regexPattern: @"^(?:\+1)?[ -]?\(?\d{3}\)?[ -]?\d{3}[ -]?\d{4}$"
    );


    public Country(string name, int value, string displayName, string phoneCode, string regexPattern)
     : base(name, value)
    {
        DisplayName = displayName;
        PhoneCode = phoneCode;
        RegexPattern = regexPattern;
    }
    public bool IsValidPhoneNumber(string normalizedNumber)
    {
        return Regex.IsMatch(normalizedNumber, RegexPattern);
    }
}



