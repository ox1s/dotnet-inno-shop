using InnoShop.ProductManagement.Domain.WishlistAggregate;
using InnoShop.ProductManagement.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoShop.ProductManagement.Infrastructure.Persistence.Configurations;

public class WishlistConfigurations : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("wishlists");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .ValueGeneratedNever();

        builder.Property(w => w.UserId).IsRequired();


        builder.Property<List<Guid>>("_productIds")
            .HasColumnName("product_ids")
            .HasListOfIdsConverter();
    }
}