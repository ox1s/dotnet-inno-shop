using Ardalis.SmartEnum;
using InnoShop.SharedKernel.Security.Permissions;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public sealed class Permission
{
    public static readonly Permission UserRead = new(1, AppPermissions.User.Read);
    public static readonly Permission UserDelete = new(2, AppPermissions.User.Delete);
    public static readonly Permission UserProfileCreate = new(3, AppPermissions.UserProfile.Create);
    public static readonly Permission UserProfileUpdate = new(4, AppPermissions.UserProfile.Update);
    public static readonly Permission UserProfileRead = new(5, AppPermissions.UserProfile.Read);
    public static readonly Permission UserProfileActivate = new(6, AppPermissions.UserProfile.Activate);
    public static readonly Permission UserProfileDeactivate = new(7, AppPermissions.UserProfile.Deactivate);
    public static readonly Permission ReviewCreate = new(8, AppPermissions.Review.Create);
    public static readonly Permission ReviewRead = new(9, AppPermissions.Review.Read);
    public static readonly Permission ReviewDelete = new(10, AppPermissions.Review.Delete);
    public static readonly Permission ReviewUpdate = new(11, AppPermissions.Review.Update);


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
        UserProfileCreate,
        UserProfileUpdate,
        UserProfileRead,
        UserProfileActivate,
        UserProfileDeactivate,
        ReviewCreate,
        ReviewRead,
        ReviewDelete,
        ReviewUpdate
    ];
}