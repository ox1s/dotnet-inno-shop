namespace InnoShop.Users.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    protected AuditableEntity(Guid id) : base(id) { }
    protected AuditableEntity() { }
}
