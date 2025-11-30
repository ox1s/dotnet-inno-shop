using ErrorOr;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public static class PriceErrors
{
    public static readonly Error InvalidPrice = Error.Validation(
        "Price.InvalidPrice",
        "Price must be positive.");
}