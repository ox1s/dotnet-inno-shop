using Ardalis.SmartEnum;

namespace InnoShop.Users.Domain.Users;

public class Role(string name, int value)
    : SmartEnum<Role>(name, value)
{
    public static readonly Role Customer = new(nameof(Customer), 0);
    public static readonly Role Admin = new(nameof(Admin), 1);

}