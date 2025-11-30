using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Api;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.BackgroundServices;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using RabbitMQ.Client;
using Constants = InnoShop.UserManagement.TestCommon.TestConstants.Constants;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Common;

public class MediatorFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    public readonly Guid DefaultUserId = Guid.NewGuid();
    private ICurrentUserProvider _currentUserProviderMock = null!;
    private SqliteTestDatabase _testDatabase = null!;

    public Task InitializeAsync()
    {
        _testDatabase = SqliteTestDatabase.CreateAndInitialize();
        return Task.CompletedTask;
    }

    public new Task DisposeAsync()
    {
        _testDatabase?.Dispose();
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__messaging", "");
        Environment.SetEnvironmentVariable("ConnectionStrings__innoshop-users", "DataSource=:memory:");
        Environment.SetEnvironmentVariable("ConnectionStrings__mailpit", "Endpoint=http://localhost:1025");
        Environment.SetEnvironmentVariable("ConnectionStrings__minio", "Endpoint=http://localhost:9000");

        // Ensure configuration is set before Program.cs reads it
        builder.ConfigureAppConfiguration(config =>
        {
            var inMemoryConfig = new Dictionary<string, string?>
            {
                { "ConnectionStrings:innoshop-users", "DataSource=:memory:" },
                { "ConnectionStrings:messaging", "" },
                { "ConnectionStrings:mailpit", "Endpoint=http://localhost:1025" },
                { "ConnectionStrings:minio", "Endpoint=http://localhost:9000" }
            };
            config.AddInMemoryCollection(inMemoryConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<PublishIntegrationEventsBackgroundService>();
            services.RemoveAll<ConsumeIntegrationEventsBackgroundService>();

            services.RemoveAll<IConnectionFactory>();
            services.AddSingleton<IConnectionFactory>(_ => Substitute.For<IConnectionFactory>());

            if (_testDatabase?.Connection == null)
                throw new InvalidOperationException("_testDatabase is not initialized.");

            services.RemoveAll<DbContextOptions<UserManagementDbContext>>();
            services.AddDbContext<UserManagementDbContext>(options =>
                options.UseSqlite(_testDatabase.Connection)
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            services.RemoveAll<IEmailSender>();
            services.AddScoped(_ => Substitute.For<IEmailSender>());

            services.RemoveAll<IFileStorage>();
            services.AddScoped(_ => Substitute.For<IFileStorage>());

            services.RemoveAll<IEmailVerificationLinkFactory>();
            var linkFactory = Substitute.For<IEmailVerificationLinkFactory>();
            linkFactory.Create(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(callInfo =>
                    $"http://localhost:5000/verify?userId={callInfo.ArgAt<Guid>(0)}&token={callInfo.ArgAt<string>(1)}");
            linkFactory.CreateResetPasswordLink(Arg.Any<string>(), Arg.Any<string>())
                .Returns(callInfo =>
                    $"http://localhost:5173/reset-password?email={callInfo.ArgAt<string>(0)}&token={callInfo.ArgAt<string>(1)}");
            services.AddScoped(_ => linkFactory);

            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();

            services.RemoveAll<ICurrentUserProvider>();
            _currentUserProviderMock = Substitute.For<ICurrentUserProvider>();


            var allPermissions = new List<string>
            {
                AppPermissions.User.Create, AppPermissions.User.Read, AppPermissions.User.Delete,
                AppPermissions.Review.Create, AppPermissions.Review.Read, AppPermissions.Review.Update,
                AppPermissions.Review.Delete,
                AppPermissions.UserProfile.Create, AppPermissions.UserProfile.Read, AppPermissions.UserProfile.Update,
                AppPermissions.UserProfile.Activate, AppPermissions.UserProfile.Deactivate
            };
            _currentUserProviderMock.GetCurrentUser().Returns(new CurrentUser(
                DefaultUserId,
                "test@test.com",
                allPermissions,
                new List<string> { AppRoles.Admin, AppRoles.Seller, AppRoles.Registered }
            ));

            ResetCurrentUser();

            // currentUserProvider.GetCurrentUser().Returns(new CurrentUser(DefaultUserId, "test@test.com", Array.Empty<string>(), Array.Empty<string>()));
            services.AddScoped(_ => _currentUserProviderMock);
        });
    }

    public void ResetDatabase()
    {
        _testDatabase.ResetDatabase();

        using var scope = Services.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        var userIdsToClear = new[]
        {
            DefaultUserId,
            Constants.Review.AuthorId,
            Constants.Review.TargetUserId
        };

        foreach (var userId in userIdsToClear)
        {
            cache.Remove($"auth:permissions-{userId}");
            cache.Remove($"auth:roles-{userId}");
        }

        ResetCurrentUser();
    }

    private void ResetCurrentUser()
    {
        var allPermissions = new List<string>
        {
            AppPermissions.User.Create, AppPermissions.User.Read, AppPermissions.User.Delete,
            AppPermissions.Review.Create, AppPermissions.Review.Read, AppPermissions.Review.Update,
            AppPermissions.Review.Delete,
            AppPermissions.UserProfile.Create, AppPermissions.UserProfile.Read, AppPermissions.UserProfile.Update,
            AppPermissions.UserProfile.Activate, AppPermissions.UserProfile.Deactivate
        };

        _currentUserProviderMock.GetCurrentUser().Returns(new CurrentUser(
            DefaultUserId,
            "test@test.com",
            allPermissions,
            new List<string> { AppRoles.Admin, AppRoles.Seller, AppRoles.Registered }
        ));
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
}