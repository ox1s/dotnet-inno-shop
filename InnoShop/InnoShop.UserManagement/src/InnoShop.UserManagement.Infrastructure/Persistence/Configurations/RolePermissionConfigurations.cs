using InnoShop.UserManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoShop.UserManagement.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });

        builder.HasData(
            // Registered - базовые права
            new RolePermission
            {
                RoleId = Role.Registered.Id,
                PermissionId = Permission.UserRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Registered.Id,
                PermissionId = Permission.UserProfileRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Registered.Id,
                PermissionId = Permission.ReviewRead.Id
            },

            // Verified - может создать профиль
            new RolePermission
            {
                RoleId = Role.Verified.Id,
                PermissionId = Permission.UserProfileCreate.Id
            },

            // Seller - полные права на отзывы и профиль
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewCreate.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewDelete.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.UserProfileUpdate.Id
            },

            // Admin - полные права
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.UserDelete.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.UserProfileActivate.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.UserProfileDeactivate.Id
            }
        );
    }
}