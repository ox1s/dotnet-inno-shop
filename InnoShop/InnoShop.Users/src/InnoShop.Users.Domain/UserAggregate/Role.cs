using Ardalis.SmartEnum;

namespace InnoShop.Users.Domain.UserAggregate;

public class Role(string name, int value)
    : SmartEnum<Role>(name, value)
{
    public static readonly Role User = new(nameof(User), 0);
    public static readonly Role Admin = new(nameof(Admin), 1);

}