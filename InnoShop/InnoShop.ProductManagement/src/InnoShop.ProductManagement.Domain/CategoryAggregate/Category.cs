using InnoShop.SharedKernel.Common;

namespace InnoShop.ProductManagement.Domain.CategoryAggregate;

public sealed class Category : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public Guid? ParentId { get; private set; }

    private Category(Guid id) : base(id) { }

    public static Category Create(Guid id, string name, Guid? parentId = null)
    {
        return new Category(id)
        {
            Name = name,
            ParentId = parentId
        };
    }

    private Category() { }
}
