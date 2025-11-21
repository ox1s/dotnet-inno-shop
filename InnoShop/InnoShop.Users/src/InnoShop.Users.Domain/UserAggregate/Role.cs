using Ardalis.SmartEnum;

namespace InnoShop.Users.Domain.UserAggregate;

public class Role : SmartEnum<Role>
{
    public static readonly Role User = new(nameof(User), 0);
    public static readonly Role Admin = new(nameof(Admin), 1);

    public Role(string name, int value) : base(name, value)
    {
    }

}