namespace InnoShop.Users.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; init; }

    public override bool Equals(object? other)
    {
        if (other is not Entity otherAsEntity)
        {
            return false;
        }
        return otherAsEntity.Id == Id;
    }

    protected Entity(Guid id) => Id = id;
    public override int GetHashCode() => Id.GetHashCode();
    protected Entity() { }

}
