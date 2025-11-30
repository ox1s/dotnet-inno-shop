using InnoShop.SharedKernel.Common;

namespace InnoShop.ProductManagement.Domain.WishlistAggregate;

public sealed class Wishlist : AggregateRoot
{
    public Guid UserId { get; private set; }
    private readonly List<Guid> _productIds = new();
    public IReadOnlyList<Guid> ProductIds => _productIds.ToList();

    private Wishlist(Guid id) : base(id) { }

    public static Wishlist Create(Guid id, Guid userId)
    {
        return new Wishlist(id)
        {
            UserId = userId
        };
    }

    public void AddProduct(Guid productId)
    {
        if (!_productIds.Contains(productId))
        {
            _productIds.Add(productId);
        }
    }

    public void RemoveProduct(Guid productId)
    {
        _productIds.Remove(productId);
    }

    private Wishlist() { }
}
