using ErrorOr;
using System.Text.RegularExpressions;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record FirstName(string Value) : NameValueObject(Value)
{
    public static ErrorOr<FirstName> Create(string value)
    {
        return Validate(value, nameof(FirstName))
                .Then(v => new FirstName(v));
    }
}