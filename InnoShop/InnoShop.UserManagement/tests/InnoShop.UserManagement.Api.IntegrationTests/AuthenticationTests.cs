using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FluentAssertions;
using InnoShop.UserManagement.Contracts.Authentication;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace InnoShop.UserManagement.Api.IntegrationTests.Tests;

public class AuthenticationTests : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(300);
    private DistributedApplication? _app;
    private HttpClient? _httpClient;

    public async Task InitializeAsync()
    {
        var cancellationToken = CancellationToken.None;

        // Create testing builder based on AppHost
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.InnoShop_AppHost>(cancellationToken);
        appHost.AddRedis("cache");
        
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await appHost.BuildAsync(cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        await _app.StartAsync(cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        // Wait for resources to be healthy
        await _app.ResourceNotifications
            .WaitForResourceHealthyAsync("innoshop-users", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        await _app.ResourceNotifications
            .WaitForResourceHealthyAsync("users-api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        // Initialize database (migrations + seed roles)
        await InitializeDatabaseAsync(cancellationToken);

        // Create HttpClient for API
        _httpClient = _app.CreateHttpClient("users-api");
    }

    private async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        var connectionString = await _app!.GetConnectionStringAsync("innoshop-users", cancellationToken);

        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var dbContext = new UserManagementDbContext(options, null!, null!, null!);
        await dbContext.Database.MigrateAsync(cancellationToken);

        // Seed roles
        if (!await dbContext.Set<Role>().AnyAsync(cancellationToken))
        {
            dbContext.AttachRange(Role.List);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }

        _httpClient?.Dispose();
    }

    [Fact]
    public async Task PostRegister_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        var registerData = new { Email = "testuser@example.com", Password = "P@ssw0rd123" };
        var cancellationToken = CancellationToken.None;

        // Act: POST /authentication/register
        using var response = await _httpClient!.PostAsJsonAsync(
            "/authentication/register",
            registerData,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<AuthenticationResponse>(cancellationToken: cancellationToken);
        content.Should().NotBeNull();
        content!.Email.Should().Be("testuser@example.com");

        // Assert: Verify in database
        var connectionString = await _app!.GetConnectionStringAsync("innoshop-users", cancellationToken);
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var dbContext = new UserManagementDbContext(options, null!, null!, null!);
        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.Email.Value == "testuser@example.com",
            cancellationToken);

        user.Should().NotBeNull();
        user!.IsEmailVerified.Should().BeFalse();
    }
}