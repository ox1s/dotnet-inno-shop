using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents;
using InnoShop.UserManagement.Infrastructure.Middleware;
using InnoShop.SharedKernel.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;


namespace InnoShop.UserManagement.Infrastructure.Persistence;

public class UserManagementDbContext(
    DbContextOptions<UserManagementDbContext> options,
    IHttpContextAccessor httpContextAccessor,
    IPublisher publisher,
    ILogger<UserManagementDbContext> logger)
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    // public DbSet<Product> Products { get; set; } = null!;
    public DbSet<OutboxIntegrationEvent> OutboxIntegrationEvents { get; set; } = null!;


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

            if (Database.CurrentTransaction != null)
            {
                var result = await base.SaveChangesAsync(cancellationToken);

                if (IsUserWaitingOnline())
                {
                    AddDomainEventsToOfflineProcessingQueue(domainEvents);
                }
                else
                {
                    await PublishDomainEventsAsync(domainEvents);
                }

                return result;
            }
            else
            {
                await using var transaction = await Database.BeginTransactionAsync(cancellationToken);

                var result = await base.SaveChangesAsync(cancellationToken);

                if (IsUserWaitingOnline())
                {
                    AddDomainEventsToOfflineProcessingQueue(domainEvents);
                }
                else
                {
                    await PublishDomainEventsAsync(domainEvents);
                }

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during SaveChangesAsync");
            throw new EventualConsistencyException("Failed to publish domain events");
        }
    }
    private async Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
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
    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        if (httpContextAccessor.HttpContext is null) return;

        if (!httpContextAccessor.HttpContext.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value)
            || value is not Queue<IDomainEvent> domainEventsQueue)
        {
            domainEventsQueue = new Queue<IDomainEvent>();
        }

        foreach (var domainEvent in domainEvents)
        {
            domainEventsQueue.Enqueue(domainEvent);
        }

        httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
    }

    private bool IsUserWaitingOnline() => httpContextAccessor.HttpContext is not null;

    public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        await base.SaveChangesAsync(cancellationToken);
    }
}
