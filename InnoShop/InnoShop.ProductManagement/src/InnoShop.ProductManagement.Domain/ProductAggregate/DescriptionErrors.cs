using ErrorOr;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public static class DescriptionErrors
{
    public static readonly Error InvalidDescription = Error.Validation(
        "Product.InvalidDescription",
        "Description is required.");

    public static readonly Error InvalidDescriptionLength = Error.Validation(
        "Product.InvalidDescriptionLength",
        "Description must be between 2 and 5000 characters.");
}