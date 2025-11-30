using ErrorOr;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public static class TitleErrors
{
    public static readonly Error InvalidTitle = Error.Validation(
        "Product.InvalidTitle",
        "Title is required.");

    public static readonly Error InvalidTitleLength = Error.Validation(
        "Product.InvalidTitleLength",
        "Title must be between 2 and 500 characters.");
}