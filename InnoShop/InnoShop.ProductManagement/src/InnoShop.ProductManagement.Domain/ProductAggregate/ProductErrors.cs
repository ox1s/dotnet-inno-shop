using ErrorOr;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public static class ProductErrors
{
    public static readonly Error InvalidTitle = Error.Validation(
        "Product.InvalidTitle",
        "Title is required.");

    public static readonly Error NotFound = Error.NotFound(
        "Product.NotFound",
        "Product not found.");

    public static readonly Error Forbidden = Error.Forbidden(
        "Product.Forbidden",
        "You don't have permission to perform this action on this product.");
}
