using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.Common.EventualConsistency;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents;
using InnoShop.UserManagement.Infrastructure.Middleware;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading;

namespace InnoShop.UserManagement.Infrastructure.Persistence;

public class UserManagementDbContext : DbContext, IUnitOfWork
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;
    private readonly ILogger<UserManagementDbContext> _logger;

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    // public DbSet<Product> Products { get; set; } = null!;
    public DbSet<OutboxIntegrationEvent> OutboxIntegrationEvents { get; set; } = null!;

    public UserManagementDbContext(
        DbContextOptions options,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        ILogger<UserManagementDbContext> logger)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
        _logger = logger;
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var domainEvents = ChangeTracker.Entries<AggregateRoot>()
                .SelectMany(entry => entry.Entity.PopDomainEvents())
                .ToList();

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during SaveChangesAsync");
            throw new EventualConsistencyException("Failed to publish domain events");
        }
    }
    private async Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            try
            {
                await _publisher.Publish(domainEvent);
                _logger.LogInformation("Published domain event: {EventType}", domainEvent.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish domain event: {EventType}", domainEvent.GetType().Name);
                throw;
            }
        }
    }
    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        if (_httpContextAccessor.HttpContext is null) return;

        if (!_httpContextAccessor.HttpContext.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value)
            || value is not Queue<IDomainEvent> domainEventsQueue)
        {
            domainEventsQueue = new Queue<IDomainEvent>();
        }

        foreach (var domainEvent in domainEvents)
        {
            domainEventsQueue.Enqueue(domainEvent);
        }

        _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        await base.SaveChangesAsync(cancellationToken);
    }
}
