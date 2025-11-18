using InnoShop.Users.Domain.Common;

namespace InnoShop.Users.Domain.Admins;

public class Admin : Entity
{
    private Guid UserId { get; }

    private Admin(
        Guid userId,
        Guid id)
        : base(id)
    {
        UserId = userId;
    }

    public static Admin Create(Guid userId, Guid? id = null)
    {
        return new Admin(userId, id ?? Guid.NewGuid());
    }
}
