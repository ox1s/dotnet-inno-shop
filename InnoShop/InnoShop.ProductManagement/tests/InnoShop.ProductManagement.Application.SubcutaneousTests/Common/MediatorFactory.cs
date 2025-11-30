using ErrorOr;
using InnoShop.ProductManagement.Api;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using InnoShop.ProductManagement.Infrastructure.IntegrationEvents.BackgroundServices;
using InnoShop.ProductManagement.Infrastructure.Persistence;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Roles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using RabbitMQ.Client;

namespace InnoShop.ProductManagement.Application.SubcutaneousTests.Common;

public class MediatorFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    public readonly Guid DefaultUserId = Guid.NewGuid();
    private ICurrentUserProvider _currentUserProviderMock = null!;
    private SqliteTestDatabase _testDatabase = null!;
    private IUserGateway _userGatewayMock = null!;

    public Task InitializeAsync()
    {
        _testDatabase = SqliteTestDatabase.CreateAndInitialize();
        return Task.CompletedTask;
    }

    public new async Task DisposeAsync()
    {
        _testDatabase?.Dispose();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__messaging", "");
        Environment.SetEnvironmentVariable("ConnectionStrings__innoshop-products", "DataSource=:memory:");

        // Ensure configuration is set before Program.cs reads it
        builder.ConfigureAppConfiguration(config =>
        {
            var inMemoryConfig = new Dictionary<string, string?>
            {
                { "ConnectionStrings:innoshop-products", "DataSource=:memory:" },
                { "ConnectionStrings:messaging", "" }
            };
            config.AddInMemoryCollection(inMemoryConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ConsumeIntegrationEventsBackgroundService>();

            services.RemoveAll<IConnectionFactory>();
            services.AddSingleton<IConnectionFactory>(_ => Substitute.For<IConnectionFactory>());

            services.RemoveAll<DbContextOptions<ProductManagementDbContext>>();
            services.AddDbContext<ProductManagementDbContext>(options =>
                options.UseSqlite(_testDatabase.Connection)
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            services.RemoveAll<IUserGateway>();
            _userGatewayMock = Substitute.For<IUserGateway>();
            services.AddScoped(_ => _userGatewayMock);

            services.RemoveAll<ICurrentUserProvider>();
            _currentUserProviderMock = Substitute.For<ICurrentUserProvider>();
            ResetCurrentUser();
            services.AddScoped(_ => _currentUserProviderMock);
        });
    }

    public void ResetDatabase()
    {
        _testDatabase.ResetDatabase();

        ResetCurrentUser();
    }

    private void ResetCurrentUser()
    {
        _currentUserProviderMock.GetCurrentUser().Returns(new CurrentUser(
            DefaultUserId,
            "test@test.com",
            Array.Empty<string>(),
            Array.Empty<string>()));
    }

    public void SetCurrentUser(Guid userId, List<string>? roles = null, List<string>? permissions = null)
    {
        var effectiveRoles = roles ?? [AppRoles.Seller, AppRoles.Registered];
        var effectivePermissions = permissions ??
        [
            AppPermissions.Review.Create,
            AppPermissions.Review.Update,
            AppPermissions.Review.Delete,
            AppPermissions.UserProfile.Update
        ];

        _currentUserProviderMock.GetCurrentUser().Returns(new CurrentUser(
            userId,
            "test@test.com",
            effectivePermissions,
            effectiveRoles));
    }

    public void SetupUserGateway(Guid userId, SellerSnapshot sellerSnapshot)
    {
        _userGatewayMock.GetSellerSnapshotAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ErrorOr<SellerSnapshot>>(sellerSnapshot));
    }
}