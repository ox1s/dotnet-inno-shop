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
            new RolePermission
            {
                // Зарегистрированный может смотреть пользователей
                RoleId = Role.Registered.Id,
                PermissionId = Permission.UserRead.Id
            },
            new RolePermission
            {
                // После подтверждения email пользователь может создать профиль
                RoleId = Role.Verified.Id,
                PermissionId = Permission.UserProfileCreate.Id
            },
            new RolePermission
            {
                // Админ может удалять пользователей
                RoleId = Role.Admin.Id,
                PermissionId = Permission.UserDelete.Id
            },
            new RolePermission
            {
                // После создания профиля пользователь может создать оценку
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewCreate.Id
            },
            new RolePermission
            {
                // После создания профиля пользователь может удалять оценки
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewDelete.Id
            },
            new RolePermission
            {
                // После создания профиля пользователь может редактировать оценки
                RoleId = Role.Seller.Id,
                PermissionId = Permission.ReviewUpdate.Id
            },
            new RolePermission
            {
                // Продавец (человек с профилем) может обновлять свой профиль
                RoleId = Role.Seller.Id,
                PermissionId = Permission.UserProfileUpdate.Id
            }
        );
    }
}