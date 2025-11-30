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
            new RolePermission
            {
                RoleId = Role.Registered.Id,
                PermissionId = Permission.ProductRead.Id
            },

            // Verified - может создать профиль (все права Registered + UserProfileCreate)
            new RolePermission
            {
                RoleId = Role.Verified.Id,
                PermissionId = Permission.UserRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Verified.Id,
                PermissionId = Permission.UserProfileRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Verified.Id,
                PermissionId = Permission.ReviewRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Verified.Id,
                PermissionId = Permission.ProductRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Verified.Id,
                PermissionId = Permission.UserProfileCreate.Id
            },

            // Seller - полные права на отзывы, профиль и продукты (все права Verified + дополнительные)
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.UserRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.UserProfileRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ProductRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.UserProfileCreate.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.UserProfileUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewCreate.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewDelete.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ProductCreate.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ProductUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ProductDelete.Id
            },

            // Admin - полные права (все права Seller + административные)
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.UserRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.UserDelete.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.UserProfileCreate.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.UserProfileUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.UserProfileRead.Id
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
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.ReviewCreate.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.ReviewRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.ReviewUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.ReviewDelete.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.ProductCreate.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.ProductRead.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.ProductUpdate.Id
            },
            new RolePermission
            {
                RoleId = Role.Admin.Id,
                PermissionId = Permission.ProductDelete.Id
            }
        );
    }
}
