using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Infrastructure.EmailService;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.BackgroundServices;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.Settings;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.Infrastructure.Persistence.Repositories;
using InnoShop.UserManagement.Infrastructure.Security;
using InnoShop.UserManagement.Infrastructure.Security.PasswordHasher;
using InnoShop.UserManagement.Infrastructure.Security.PolicyEnforcer;
using InnoShop.UserManagement.Infrastructure.Security.TokenGenerator;
using InnoShop.UserManagement.Infrastructure.Security.TokenValidation;
using InnoShop.UserManagement.Infrastructure.Services;
using InnoShop.UserManagement.Infrastructure.Storage;


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mail;
using Throw;
using ICustomAuthorizationService = InnoShop.UserManagement.Application.Common.Interfaces.IAuthorizationService;


namespace InnoShop.UserManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpContextAccessor()
            .AddMediatR()
            .AddConfigurations(configuration)
            .AddBackgroundServices()
            .AddPersistence(configuration)
            .AddAuthentication()
            .AddAuthorization()
            .AddEmailService(configuration)
            .AddServices()
            .AddStorage();

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
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.Section));
        services.Configure<BlobStorageSettings>(configuration.GetSection(BlobStorageSettings.Section));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddSingleton<IIntegrationEventsPublisher, IntegrationEventsPublisher>();
        services.AddHostedService<PublishIntegrationEventsBackgroundService>();
        services.AddHostedService<ConsumeIntegrationEventsBackgroundService>();

        return services;
    }
    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IEmailVerificationLinkFactory, EmailVerificationLinkFactory>();

        var mailPitConnectionString = configuration.GetConnectionString("mailpit");

        mailPitConnectionString.ThrowIfNull();
        mailPitConnectionString = mailPitConnectionString.Replace("Endpoint=", "");


        var uri = new Uri(mailPitConnectionString, UriKind.Absolute);

        var host = uri.Host;
        var port = uri.Port;



        var emailSettings = configuration.GetSection(EmailSettings.Section).Get<EmailSettings>() ?? new EmailSettings();

        services
            .AddFluentEmail(emailSettings.FromEmail, emailSettings.FromName)
            .AddSmtpSender(new SmtpClient(host, port));

        services.AddTransient<IEmailSender, EmailSender>();

        return services;

    }
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("innoshop-users")
            ?? throw new InvalidOperationException("Connection string 'innoshop-users' not found"); ;

        if (connectionString.Contains("DataSource", StringComparison.OrdinalIgnoreCase) ||
        connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<UserManagementDbContext>(options =>
                options.UseSqlite(connectionString));
        }
        else
        {
            services.AddDbContext<UserManagementDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UserManagementDbContext>());
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IReviewsRepository, ReviewsRepository>();

        return services;
    }
    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<ICustomAuthorizationService, AuthorizationService>();
        services.AddSingleton<IPolicyEnforcer, PolicyEnforcer>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services
            .ConfigureOptions<JwtBearerTokenValidationConfiguration>()
            .AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;

    }

    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddScoped<IFileStorage, MinioFileStorage>();
        return services;

    }
}