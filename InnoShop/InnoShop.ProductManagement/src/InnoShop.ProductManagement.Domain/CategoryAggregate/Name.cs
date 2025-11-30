using System.Text.RegularExpressions;
using ErrorOr;

namespace InnoShop.ProductManagement.Domain.CategoryAggregate;

public sealed record Name(string Value)
{
    public static ErrorOr<Name> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return CategoryErrors.InvalidName;

        if (value.Length < 2 || value.Length > 200) return CategoryErrors.InvalidNameLength;

        if (!Regex.IsMatch(value, @"^[a-zA-Zа-яА-ЯёЁ\- ]+$")) return CategoryErrors.InvalidNameChars;
        return new Name(value);
    }
}