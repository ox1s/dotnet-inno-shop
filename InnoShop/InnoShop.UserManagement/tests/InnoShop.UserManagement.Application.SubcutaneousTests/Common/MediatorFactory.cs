using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Common;

public class MediatorFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private SqliteTestDatabase _testDatabase = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _testDatabase = SqliteTestDatabase.CreateAndInitialize();

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:innoshop-users"] = "DataSource=:memory:",
                ["ConnectionStrings:mailpit"] = "Endpoint=http://localhost:1025",
                ["ConnectionStrings:minio"] = "Endpoint=http://localhost:9000",
                ["ConnectionStrings:messaging"] = "amqp://guest:guest@localhost:5672"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<DbContextOptions<UserManagementDbContext>>()
                .AddDbContext<UserManagementDbContext>((sp, options) => options.UseSqlite(_testDatabase.Connection));

            services.RemoveAll<IEmailSender>();
            services.AddScoped(_ => Substitute.For<IEmailSender>());

            services.RemoveAll<IFileStorage>();
            services.AddScoped(_ => Substitute.For<IFileStorage>());
        });
    }

    public IMediator CreateMediator()
    {
        var serviceScope = Services.CreateScope();
        _testDatabase.ResetDatabase();
        return serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public new Task DisposeAsync()
    {
        _testDatabase.Dispose();
        return Task.CompletedTask;
    }
}
