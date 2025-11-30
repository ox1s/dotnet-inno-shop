using InnoShop.SharedKernel.Common;

namespace InnoShop.ProductManagement.Domain.CategoryAggregate;

public sealed class Category : AggregateRoot
{
    private Category(Guid id, Name name, Guid? parentId = null) : base(id)
    {
        Name = name;
        ParentId = parentId;
    }

    private Category()
    {
    }

    public Name Name { get; private set; } = null!;
    public Guid? ParentId { get; private set; }

    public static Category Create(Guid id, Name name, Guid? parentId = null)
    {
        return new Category(id, name, parentId);
    }
}