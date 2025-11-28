namespace InnoShop.UserManagement.Domain.Common;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    protected AggregateRoot() { }

    protected readonly List<IDomainEvent> DomainEvents = new();

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = DomainEvents.ToList();
        DomainEvents.Clear();

        return copy;
    }
}