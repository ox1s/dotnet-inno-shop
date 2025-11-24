namespace InnoShop.UserManagement.Domain.Common;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    protected AggregateRoot() { }

    protected readonly List<IDomainEvent> _domainEvents = new();

    public List<IDomainEvent> Pop_domainEvents()
    {
        var copy = _domainEvents;
        _domainEvents.Clear();

        return copy;
    }
}