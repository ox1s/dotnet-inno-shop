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
        try
        {
            var domainEvents = ChangeTracker.Entries<AggregateRoot>()
                .SelectMany(entry => entry.Entity.PopDomainEvents())
                .ToList();

            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);

            var result = await base.SaveChangesAsync(cancellationToken);

            if (domainEvents.Any()) await PublishDomainEventsAsync(domainEvents);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during SaveChangesAsync");
            throw;
        }
    }

    private async Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
            try
            {
                await publisher.Publish(domainEvent);
                logger.LogInformation("Published domain event: {EventType}", domainEvent.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish domain event: {EventType}", domainEvent.GetType().Name);
                throw;
            }
    }
}