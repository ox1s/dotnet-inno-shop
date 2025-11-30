using ErrorOr;

namespace InnoShop.ProductManagement.Domain.CategoryAggregate;

public static class CategoryErrors
{
    public static readonly Error InvalidName = Error.Validation(
        "Category.InvalidName",
        "Name must be between 2 and 200 characters and can only contain letters, hyphens, or spaces.");

    public static readonly Error InvalidNameLength = Error.Validation(
        "Category.InvalidNameLength",
        "Name must be between 2 and 200 characters.");

    public static readonly Error InvalidNameChars = Error.Validation(
        "Category.InvalidNameChars",
        "Name can only contain letters, hyphens, or spaces.");
}