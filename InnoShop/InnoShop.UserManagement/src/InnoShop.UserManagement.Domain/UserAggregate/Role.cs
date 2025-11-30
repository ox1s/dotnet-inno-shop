using InnoShop.SharedKernel.Security.Roles;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public sealed class Role
{
    public static readonly Role Registered = new(1, AppRoles.Registered);
    public static readonly Role Admin = new(2, AppRoles.Admin);
    public static readonly Role Verified = new(3, AppRoles.Verified);
    public static readonly Role Seller = new(4, AppRoles.Seller);


    private Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; }


    public ICollection<Permission> Permissions { get; init; } = new List<Permission>();

    public static IReadOnlyList<Role> List =>
    [
        Registered,
        Admin,
        Verified,
        Seller
    ];
}