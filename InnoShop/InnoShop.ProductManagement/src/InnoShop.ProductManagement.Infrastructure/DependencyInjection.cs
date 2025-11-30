using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Infrastructure.IntegrationEvents.BackgroundServices;
using InnoShop.ProductManagement.Infrastructure.IntegrationEvents.Settings;
using InnoShop.ProductManagement.Infrastructure.Persistence;
using InnoShop.ProductManagement.Infrastructure.Persistence.Repositories;
using InnoShop.ProductManagement.Infrastructure.Security;
using InnoShop.ProductManagement.Infrastructure.Security.TokenValidation;
using InnoShop.ProductManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.ProductManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpContextAccessor()
            .AddConfigurations(configuration)
            .AddBackgroundServices()
            .AddPersistence(configuration)
            .AddAuthentication()
            .AddAuthorization();

        return services;
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.Section));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<ConsumeIntegrationEventsBackgroundService>();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("innoshop-products")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'innoshop-products' not found");

        if (connectionString.Contains("DataSource", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
            services.AddDbContext<ProductManagementDbContext>(options =>
                options.UseSqlite(connectionString));
        else
            services.AddDbContext<ProductManagementDbContext>(options =>
                options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProductManagementDbContext>());
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<IUserGateway, UserGateway>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        services
            .ConfigureOptions<JwtBearerTokenValidationConfiguration>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }
}