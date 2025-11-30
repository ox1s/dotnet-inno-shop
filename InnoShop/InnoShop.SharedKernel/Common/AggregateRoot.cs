namespace InnoShop.SharedKernel.Common;

public abstract class AggregateRoot : Entity
{
    protected readonly List<IDomainEvent> DomainEvents = new();

    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    protected AggregateRoot()
    {
    }

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = DomainEvents.ToList();
        DomainEvents.Clear();

        return copy;
    }
}