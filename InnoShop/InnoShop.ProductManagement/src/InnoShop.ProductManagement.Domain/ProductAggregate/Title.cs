using ErrorOr;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public sealed record Title(string Value)
{
    public static ErrorOr<Title> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return TitleErrors.InvalidTitle;

        if (value.Length < 2 || value.Length > 500) return TitleErrors.InvalidTitleLength;
        return new Title(value);
    }
}