using ErrorOr;
using InnoShop.SharedKernel.Common;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public sealed class Product : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Price Price { get; private set; } = null!;
    public Guid SellerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public SellerSnapshot SellerInfo { get; private set; } = null!;

    private readonly List<Image> _images = new();
    public IReadOnlyList<Image> Images => _images.ToList();

    private Product(Guid id) : base(id) { }

    public static Product CreateProduct(
        Guid id,
        string title,
        string description,
        Price price,
        Guid sellerId,
        SellerSnapshot sellerInfo,
        IEnumerable<Image>? images = null)
    {
        var product = new Product(id)
        {
            Title = title,
            Description = description,
            Price = price,
            SellerId = sellerId,
            SellerInfo = sellerInfo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (images != null)
        {
            product._images.AddRange(images);
        }

        return product;
    }

    public ErrorOr<Success> Update(string title, string description, Price price)
    {
        if (string.IsNullOrWhiteSpace(title)) return ProductErrors.InvalidTitle;

        Title = title;
        Description = description;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    public void AddImage(Image image)
    {
        if (!_images.Contains(image))
        {
            _images.Add(image);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveImage(Image image)
    {
        _images.Remove(image);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Hide()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSellerInfo(SellerSnapshot newInfo)
    {
        SellerInfo = newInfo;
        UpdatedAt = DateTime.UtcNow;
    }

    private Product() { }
}