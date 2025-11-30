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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using RabbitMQ.Client;

namespace InnoShop.UserManagement.Api.IntegrationTests.Common;

public class ApiFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    private SqliteTestDatabase _testDatabase = null!;
    public HttpClient HttpClient { get; private set; } = null!;

    public Task InitializeAsync()
    {
        _testDatabase = SqliteTestDatabase.CreateAndInitialize();
        HttpClient = CreateClient();
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Отключаем переменные Aspire, чтобы Program.cs не пытался подключиться к реальным сервисам
        Environment.SetEnvironmentVariable("ConnectionStrings__innoshop-users", "DataSource=:memory:");
        Environment.SetEnvironmentVariable("ConnectionStrings__messaging", "amqp://guest:guest@localhost:5672"); // Фейковая строка
        Environment.SetEnvironmentVariable("ConnectionStrings__mailpit", "Endpoint=http://localhost:1025");
        Environment.SetEnvironmentVariable("ConnectionStrings__minio", "Endpoint=http://localhost:9000");

        builder.ConfigureTestServices(services =>
        {
            // Инициализируем базу данных, если еще не инициализирована
            if (_testDatabase == null)
            {
                _testDatabase = SqliteTestDatabase.CreateAndInitialize();
            }

            // --- 1. База Данных (SQLite) ---
            services.RemoveAll<DbContextOptions<UserManagementDbContext>>();
            services.AddDbContext<UserManagementDbContext>(options =>
                options.UseSqlite(_testDatabase.Connection)
                       .ConfigureWarnings(x => x.Ignore(RelationalEventId.PendingModelChangesWarning)));

            // --- 2. RabbitMQ (Убираем Background Services и мокаем соединение) ---
            // Убираем сервисы, которые слушают очередь (иначе приложение упадет при старте)
            services.RemoveAll<PublishIntegrationEventsBackgroundService>();
            services.RemoveAll<ConsumeIntegrationEventsBackgroundService>();

            // Мокаем фабрику соединений RabbitMQ, чтобы DI контейнер собрался
            services.RemoveAll<IConnectionFactory>();
            services.AddSingleton(_ => Substitute.For<IConnectionFactory>());
            services.RemoveAll<IConnection>();
            services.AddSingleton(_ => Substitute.For<IConnection>());

            // --- 3. Redis (Заменяем на In-Memory Cache) ---
            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();

            // --- 4. MinIO (Мокаем хранилище файлов) ---
            services.RemoveAll<IFileStorage>();
            var mockStorage = Substitute.For<IFileStorage>();
            mockStorage.UploadAsync(default, default, default, default).ReturnsForAnyArgs("http://mock-url.com/avatar.jpg");
            services.AddScoped(_ => mockStorage);

            // --- 5. Email (Мокаем отправку) ---
            services.RemoveAll<IEmailSender>();
            services.AddScoped(_ => Substitute.For<IEmailSender>());

            // --- 6. Email Verification Link Factory (Мокаем генерацию ссылок) ---
            services.RemoveAll<IEmailVerificationLinkFactory>();
            var linkFactory = Substitute.For<IEmailVerificationLinkFactory>();
            linkFactory.Create(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(callInfo =>
                    $"http://localhost:5000/verify?userId={callInfo.ArgAt<Guid>(0)}&token={callInfo.ArgAt<string>(1)}");
            linkFactory.CreateResetPasswordLink(Arg.Any<string>(), Arg.Any<string>())
                .Returns(callInfo =>
                    $"http://localhost:5173/reset-password?email={callInfo.ArgAt<string>(0)}&token={callInfo.ArgAt<string>(1)}");
            services.AddScoped(_ => linkFactory);
        });
    }

    public void ResetDatabase()
    {
        _testDatabase.ResetDatabase();
    }

    public UserManagementDbContext CreateDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
    }

    public new Task DisposeAsync()
    {
        _testDatabase.Dispose();
        return Task.CompletedTask;
    }
}
