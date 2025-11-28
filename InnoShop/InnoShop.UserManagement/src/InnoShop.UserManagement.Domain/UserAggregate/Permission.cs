using Ardalis.SmartEnum;
using InnoShop.SharedKernel.Security.Permissions;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public sealed class Permission
{
    public static readonly Permission UserRead = new(1, AppPermissions.User.Read);
    public static readonly Permission UserDelete = new(2, AppPermissions.User.Delete);
    public static readonly Permission UserProfileCreate = new(3, AppPermissions.UserProfile.Create);
    public static readonly Permission UserProfileUpdate = new(4, AppPermissions.UserProfile.Update);
    public static readonly Permission ReviewCreate = new(5, AppPermissions.Review.Create);
    public static readonly Permission ReviewDelete = new(6, AppPermissions.Review.Delete);
    public static readonly Permission ReviewUpdate = new(7, AppPermissions.Review.Update);


    private Permission(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; }


    public static IReadOnlyList<Permission> List =>
    [
        UserRead,
        UserDelete,
        UserProfileUpdate,
        UserProfileCreate,
        UserProfileUpdate,
        ReviewCreate,
        ReviewDelete,
        ReviewUpdate
    ];
}