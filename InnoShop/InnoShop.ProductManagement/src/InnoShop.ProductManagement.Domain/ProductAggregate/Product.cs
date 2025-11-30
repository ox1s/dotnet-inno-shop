using ErrorOr;
using InnoShop.SharedKernel.Common;

namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public sealed class Product : AggregateRoot
{
    private readonly List<Image> _images = new();

    private Product(Guid id) : base(id)
    {
    }

    private Product()
    {
    }

    public Title Title { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public Price Price { get; private set; } = null!;
    public Guid SellerId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public SellerSnapshot SellerInfo { get; private set; } = null!;
    public IReadOnlyList<Image> Images => _images.ToList();

    public static Product CreateProduct(
        Guid id,
        Title title,
        Description description,
        Price price,
        Guid sellerId,
        SellerSnapshot sellerInfo,
        Guid? categoryId = null,
        IEnumerable<Image>? images = null)
    {
        var product = new Product(id)
        {
            Title = title,
            Description = description,
            Price = price,
            SellerId = sellerId,
            CategoryId = categoryId,
            SellerInfo = sellerInfo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (images != null) product._images.AddRange(images);

        return product;
    }

    public ErrorOr<Success> Update(Title title, Description description, Price price)
    {
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
}