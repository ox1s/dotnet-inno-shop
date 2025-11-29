using InnoShop.UserManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoShop.UserManagement.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(permission => permission.Id);

        builder.HasData(
            Permission.UserRead,
            Permission.UserDelete,
            Permission.UserProfileCreate,
            Permission.UserProfileUpdate,
            Permission.UserProfileRead,
            Permission.UserProfileActivate,
            Permission.UserProfileDeactivate,
            Permission.ReviewCreate,
            Permission.ReviewRead,
            Permission.ReviewDelete,
            Permission.ReviewUpdate);
    }
}