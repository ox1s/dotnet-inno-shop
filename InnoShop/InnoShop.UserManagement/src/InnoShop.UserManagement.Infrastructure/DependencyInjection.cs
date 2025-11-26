using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.BackgroundServices;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.Settings;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace InnoShop.UserManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
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

        var messageBrokerSettings = new MessageBrokerSettings();
        configuration.Bind(MessageBrokerSettings.Section, messageBrokerSettings);

        services.AddSingleton(Options.Create(messageBrokerSettings));

        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddSingleton<IIntegrationEventsPublisher, IntegrationEventsPublisher>();
        services.AddHostedService<PublishIntegrationEventsBackgroundService>();
        services.AddHostedService<ConsumeIntegrationEventsBackgroundService>();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("inno-shop-users");

        services.AddDbContext<UserManagementDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IReviewsRepository, ReviewsRepository>();

        return services;
    }

}