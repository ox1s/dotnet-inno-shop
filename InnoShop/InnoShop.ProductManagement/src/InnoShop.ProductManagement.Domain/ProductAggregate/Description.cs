using ErrorOr;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public sealed record Description(string Value)
{
    public static ErrorOr<Description> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return DescriptionErrors.InvalidDescription;

        if (value.Length < 2 || value.Length > 5000) return DescriptionErrors.InvalidDescriptionLength;
        return new Description(value);
    }
}