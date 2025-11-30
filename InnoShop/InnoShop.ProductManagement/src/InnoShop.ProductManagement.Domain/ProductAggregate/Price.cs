using ErrorOr;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public sealed record Price(decimal Value)
{
    public static ErrorOr<Price> Create(decimal value)
    {
        if (value <= 0)
        {
            return PriceErrors.InvalidPrice;
        }

        return new Price(value);
    }
}
