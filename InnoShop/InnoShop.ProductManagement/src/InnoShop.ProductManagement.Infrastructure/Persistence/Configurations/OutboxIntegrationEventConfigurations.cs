using InnoShop.ProductManagement.Infrastructure.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoShop.ProductManagement.Infrastructure.Persistence.Configurations;

public class OutboxIntegrationEventConfigurations : IEntityTypeConfiguration<OutboxIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent> builder)
    {
        builder
            .Property<int>("id")
            .ValueGeneratedOnAdd();

        builder.HasKey("id");

        builder.Property(o => o.EventName);

        builder.Property(o => o.EventContent);
    }
}
