using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InnoShop.UserManagement.Api.IntegrationTests;

/// <summary>
/// Shared fixture that starts the Aspire AppHost once for all tests in the collection.
/// Implements IAsyncLifetime to manage the lifecycle of the distributed application.
/// </summary>
public class AspireAppFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(120);

    public DistributedApplication App { get; private set; } = null!;
    public HttpClient UsersApiClient { get; private set; } = null!;
    public HttpClient ProductsApiClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Create the testing builder with configuration to disable dashboard for faster CI execution
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.InnoShop_AppHost>(
                [
                    "DcpPublisher:RandomizePorts=false",
                    "--environment=Testing"
                ]);

        // Configure logging for better test diagnostics
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Information);
            logging.AddFilter("Aspire.", LogLevel.Warning);
            logging.AddFilter("Microsoft", LogLevel.Warning);
        });

        // Configure HTTP client defaults with resilience
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        // Build the distributed application
        App = await appHost.BuildAsync().WaitAsync(DefaultTimeout);

        // Start the application
        await App.StartAsync().WaitAsync(DefaultTimeout);

        // Wait for all critical resources to be healthy before proceeding with tests
        using var cts = new CancellationTokenSource(DefaultTimeout);

        // Wait for database to be ready
        await App.ResourceNotifications.WaitForResourceHealthyAsync("innoshop-users", cts.Token);

        // Wait for API services to be healthy
        await App.ResourceNotifications.WaitForResourceHealthyAsync("users-api", cts.Token);
        await App.ResourceNotifications.WaitForResourceHealthyAsync("products-api", cts.Token);

        // Create HttpClient instances for each service
        UsersApiClient = App.CreateHttpClient("users-api");
        ProductsApiClient = App.CreateHttpClient("products-api");

        // Initialize database (migrations + seed roles)
        await InitializeDatabaseAsync(cts.Token);
    }

    private async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        var connectionString = await App.GetConnectionStringAsync("innoshop-users", cancellationToken);

        var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<InnoShop.UserManagement.Infrastructure.Persistence.UserManagementDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var dbContext = new InnoShop.UserManagement.Infrastructure.Persistence.UserManagementDbContext(
            options,
            null!,
            null!,
            null!);

        await dbContext.Database.MigrateAsync(cancellationToken);

        // Seed roles if they don't exist
        if (!await dbContext.Set<InnoShop.UserManagement.Domain.UserAggregate.Role>().AnyAsync(cancellationToken))
        {
            dbContext.AttachRange(InnoShop.UserManagement.Domain.UserAggregate.Role.List);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DisposeAsync()
    {
        UsersApiClient?.Dispose();
        ProductsApiClient?.Dispose();
        await App.DisposeAsync();
    }
}
