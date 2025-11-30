using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InnoShop.ProductManagement.Domain.ProductAggregate;

namespace InnoShop.ProductManagement.Infrastructure.Persistence.Configurations;

public class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Title).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Description).HasMaxLength(5000);
        builder.Property(p => p.SellerId).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();

        builder.Property(p => p.Price)
            .HasConversion(
                price => price.Value,
                value => new Price(value))
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.OwnsOne(p => p.SellerInfo, si =>
        {
            si.Property(s => s.FullName)
                .HasColumnName("seller_full_name")
                .HasMaxLength(400)
                .IsRequired();

            si.Property(s => s.AvatarUrl)
                .HasColumnName("seller_avatar_url")
                .HasMaxLength(2048);

            si.Property(s => s.Rating)
                .HasColumnName("seller_rating")
                .HasPrecision(3, 2);
        });


        builder.OwnsMany(p => p.Images, image =>
        {
            image.ToTable("product_images");

            image.WithOwner().HasForeignKey("product_id");

            image.Property(i => i.Url)
                 .HasColumnName("url")
                 .HasMaxLength(2048)
                 .IsRequired();

            image.HasKey("product_id", "Url");
        });
        builder.Navigation(p => p.Images)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(p => p.IsActive);
    }
}
