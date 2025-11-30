namespace InnoShop.SharedKernel.Common;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
    }

    public Guid Id { get; init; }

    public override bool Equals(object? other)
    {
        if (other is not Entity otherAsEntity) return false;
        return otherAsEntity.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}