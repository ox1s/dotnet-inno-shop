using InnoShop.SharedKernel.Common;

namespace InnoShop.ProductManagement.Domain.WishlistAggregate;

public sealed class Wishlist : AggregateRoot
{
    private readonly List<Guid> _productIds = new();

    private Wishlist(Guid id) : base(id)
    {
    }

    private Wishlist()
    {
    }

    public Guid UserId { get; private set; }
    public IReadOnlyList<Guid> ProductIds => _productIds.ToList();

    public static Wishlist Create(Guid id, Guid userId)
    {
        return new Wishlist(id)
        {
            UserId = userId
        };
    }

    public void AddProduct(Guid productId)
    {
        if (!_productIds.Contains(productId)) _productIds.Add(productId);
    }

    public void RemoveProduct(Guid productId)
    {
        _productIds.Remove(productId);
    }
}