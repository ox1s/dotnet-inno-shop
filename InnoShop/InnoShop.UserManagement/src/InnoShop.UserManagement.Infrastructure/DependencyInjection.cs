using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Infrastructure.Authentication;
using InnoShop.UserManagement.Infrastructure.Authentication.CurrentUserProvider;
using InnoShop.UserManagement.Infrastructure.Authentication.PasswordHasher;
using InnoShop.UserManagement.Infrastructure.Authentication.TokenGenerator;
using InnoShop.UserManagement.Infrastructure.EmailService;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.BackgroundServices;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.Settings;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.Infrastructure.Persistence.Repositories;
using InnoShop.UserManagement.Infrastructure.Services;
using InnoShop.UserManagement.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Text;
using Throw;
namespace InnoShop.UserManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMediatR()
            .AddConfigurations(configuration)
            .AddBackgroundServices()
            .AddPersistence(configuration)
            .AddAuthentication(configuration)
            .AddServices(configuration);

        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

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
        string? connectionString = configuration.GetConnectionString("innoshop-users");

        services.AddDbContext<UserManagementDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UserManagementDbContext>());
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IReviewsRepository, ReviewsRepository>();

        return services;
    }
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.Section, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            });

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddTransient<IEmailVerificationLinkFactory, EmailVerificationLinkFactory>();

        // Mailpit ---
        var mailPitConnectionString = configuration.GetConnectionString("mailpit");

        mailPitConnectionString.ThrowIfNull();

        string host = "localhost";
        int port = 1025;

        if (mailPitConnectionString.StartsWith("Endpoint="))
        {
            mailPitConnectionString = mailPitConnectionString.Replace("Endpoint=", "");
        }

        if (Uri.TryCreate(mailPitConnectionString, UriKind.Absolute, out var uri))
        {
            host = uri.Host;
            port = uri.Port;
        }


        var smtpClient = new SmtpClient(host, port);

        services
                .AddFluentEmail("no-reply@innoshop.com", "InnoShop Cyberbot")
                .AddSmtpSender(new SmtpClient(host, port));

        services.AddTransient<IEmailSender, EmailSender>();


        // Minio ----
        services.AddScoped<IFileStorage, MinioFileStorage>();

        var minioConnectionString = configuration.GetConnectionString("minio");

        minioConnectionString.ThrowIfNull();
        services.AddScoped<IFileStorage, MinioFileStorage>();


        return services;
    }
}