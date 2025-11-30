using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoShop.UserManagement.Infrastructure.Persistence.Configurations;

public class ReviewConfigurations : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.TargetUserId)
            .HasColumnName("target_user_id")
            .IsRequired();

        builder.Property(r => r.AuthorId)
            .HasColumnName("author_id")
            .IsRequired();

        builder.Property(review => review.Rating)
            .HasConversion(
                rating => rating.Value,
                value => Rating.Create(value).Value)
            .HasColumnName("rating")
            .IsRequired();

        builder.Property(review => review.Comment)
            .HasConversion(
                comment => comment != null ? comment.Value : null,
                value => value != null ? Comment.Create(value).Value : null)
            .HasMaxLength(1000)
            .HasColumnName("comment")
            .IsRequired(false);

        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
        builder.Property(r => r.IsDeleted).HasColumnName("is_deleted");
        builder.Property(r => r.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(r => new { r.AuthorId, r.TargetUserId })
            .IsUnique()
            .HasFilter("\"is_deleted\" = false");
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}