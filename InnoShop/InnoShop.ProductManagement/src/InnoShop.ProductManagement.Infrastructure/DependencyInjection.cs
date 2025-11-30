using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Infrastructure.IntegrationEvents.BackgroundServices;
using InnoShop.ProductManagement.Infrastructure.IntegrationEvents.Settings;
using InnoShop.ProductManagement.Infrastructure.Persistence;
using InnoShop.ProductManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;

namespace InnoShop.ProductManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpContextAccessor()
            .AddMediatR()
            .AddConfigurations(configuration)
            .AddBackgroundServices()
            .AddPersistence(configuration);

        return services;
    }

    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

        return services;
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.Section));

        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<ConsumeIntegrationEventsBackgroundService>();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("innoshop-products")
            ?? throw new InvalidOperationException("Connection string 'innoshop-products' not found");

        if (connectionString.Contains("DataSource", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<ProductManagementDbContext>(options =>
                options.UseSqlite(connectionString));
        }
        else
        {
            services.AddDbContext<ProductManagementDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProductManagementDbContext>());
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<Application.Common.Interfaces.IUserGateway, Services.UserGateway>();

        return services;
    }
}
