using ErrorOr;
using System.Text.RegularExpressions;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public sealed record LastName(string Value) : NameValueObject(Value)
{
    public static ErrorOr<LastName> Create(string value)
    {
        return Validate(value, nameof(LastName))
                .Then(v => new LastName(v));
    }
}