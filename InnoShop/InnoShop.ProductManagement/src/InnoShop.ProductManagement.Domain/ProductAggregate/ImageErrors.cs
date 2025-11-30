using ErrorOr;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public static class ImageErrors
{
    public static readonly Error InvalidImageUrl = Error.Validation(
        "Image.InvalidImageUrl",
        "Image URL cannot be empty.");

    public static readonly Error ImageUrlTooLong = Error.Validation(
        "Image.ImageUrlTooLong",
        "Image URL must be under 2048 characters.");

    public static readonly Error InvalidImageUrlFormat = Error.Validation(
        "Image.InvalidImageUrlFormat",
        "Image URL must be a valid HTTP or HTTPS URL.");
}
