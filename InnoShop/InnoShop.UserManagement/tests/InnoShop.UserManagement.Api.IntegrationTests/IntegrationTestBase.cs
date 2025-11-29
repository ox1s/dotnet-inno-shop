using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InnoShop.UserManagement.Api.IntegrationTests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected DistributedApplication App = default!;
    protected IDistributedApplicationTestingBuilder AppHost = default!;
    protected HttpClient Api = default!;

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    public async Task InitializeAsync()
    {
        // 1. Создаём AppHost
        AppHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.InnoShop_AppHost>([
                "DcpPublisher:RandomizePorts=false",
                "--environment=Testing"
            ]);

        AppHost.Services.ConfigureHttpClientDefaults(builder =>
        {
            builder.AddStandardResilienceHandler();
        });

        // 3. Build AppHost
        App = await AppHost.BuildAsync().WaitAsync(Timeout);

        // 4. Запускаем приложение
        await App.StartAsync().WaitAsync(Timeout);

        // 5. Ждём пока API станет Healthy
        using var cts = new CancellationTokenSource(Timeout);
        await App.ResourceNotifications.WaitForResourceHealthyAsync(
            "users-api",
            cts.Token
        );

        // 6. Создаём HttpClient
        Api = App.CreateHttpClient("users-api");
    }

    public async Task DisposeAsync()
    {
        await App.DisposeAsync();
    }
}