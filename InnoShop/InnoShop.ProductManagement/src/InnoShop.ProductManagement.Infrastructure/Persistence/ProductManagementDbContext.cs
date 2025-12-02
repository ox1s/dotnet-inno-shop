using System.Reflection;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Domain.CategoryAggregate;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using InnoShop.ProductManagement.Domain.WishlistAggregate;
using InnoShop.ProductManagement.Infrastructure.IntegrationEvents;
using InnoShop.SharedKernel.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InnoShop.ProductManagement.Infrastructure.Persistence;

public class ProductManagementDbContext(
    DbContextOptions<ProductManagementDbContext> options,
    IHttpContextAccessor httpContextAccessor,
    IPublisher publisher,
    ILogger<ProductManagementDbContext> logger)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Wishlist> Wishlists { get; set; } = null!;
    public DbSet<OutboxIntegrationEvent> OutboxIntegrationEvents { get; set; } = null!;

    public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(e => e.Entity.PopDomainEvents())
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }

        logger.LogInformation("Publishing domain events");

        int result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}