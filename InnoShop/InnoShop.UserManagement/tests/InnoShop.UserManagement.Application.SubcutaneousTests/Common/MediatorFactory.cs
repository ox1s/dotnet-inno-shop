using InnoShop.UserManagement.Api;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.BackgroundServices;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Common;

public class MediatorFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    private SqliteTestDatabase _testDatabase = null!;
    public readonly Guid DefaultUserId = Guid.NewGuid();

    public Task InitializeAsync()
    {
        _testDatabase = SqliteTestDatabase.CreateAndInitialize();
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__messaging", "");
        Environment.SetEnvironmentVariable("ConnectionStrings__innoshop-users", "DataSource=:memory:");
        Environment.SetEnvironmentVariable("ConnectionStrings__mailpit", "Endpoint=http://localhost:1025");
        Environment.SetEnvironmentVariable("ConnectionStrings__minio", "Endpoint=http://localhost:9000");


        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<PublishIntegrationEventsBackgroundService>();
            services.RemoveAll<ConsumeIntegrationEventsBackgroundService>();

            services.RemoveAll<IConnectionFactory>();
            services.AddSingleton<IConnectionFactory>(_ => Substitute.For<IConnectionFactory>());

            if (_testDatabase?.Connection == null)
            {
                throw new InvalidOperationException("_testDatabase is not initialized.");
            }

            services.RemoveAll<DbContextOptions<UserManagementDbContext>>();
            services.AddDbContext<UserManagementDbContext>((sp, options) =>
                options.UseSqlite(_testDatabase.Connection)
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            services.RemoveAll<IEmailSender>();
            services.AddScoped(_ => Substitute.For<IEmailSender>());

            services.RemoveAll<IFileStorage>();
            services.AddScoped(_ => Substitute.For<IFileStorage>());

            services.RemoveAll<ICurrentUserProvider>();
            var currentUserProvider = Substitute.For<ICurrentUserProvider>();
            currentUserProvider.GetCurrentUser().Returns(new CurrentUser(DefaultUserId, Array.Empty<string>(), Array.Empty<string>()));
            services.AddScoped(_ => currentUserProvider);
        });
    }

    public void ResetDatabase()
    {
        _testDatabase.ResetDatabase();
    }

    public new Task DisposeAsync()
    {
        _testDatabase?.Dispose();
        return Task.CompletedTask;
    }
}