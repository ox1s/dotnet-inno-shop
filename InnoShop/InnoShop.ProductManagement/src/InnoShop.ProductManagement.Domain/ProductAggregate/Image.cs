using ErrorOr;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public sealed record Image(string Url)
{
    public static ErrorOr<Image> Create(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return ImageErrors.InvalidImageUrl;
        }

        if (url.Length > 2048)
        {
            return ImageErrors.ImageUrlTooLong;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return ImageErrors.InvalidImageUrlFormat;
        }

        return new Image(url);
    }
}
