using System;
using InnoShop.UserManagement.Api.Common.Interfaces;
using InnoShop.UserManagement.Api.Exceptions;
using InnoShop.UserManagement.Api.OpenApi;
using InnoShop.UserManagement.Application;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Infrastructure;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
{
    builder.AddRedisDistributedCache("cache", settings =>
    {
        settings.DisableTracing = false;
    });

    builder.Services.AddControllers();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

    builder.AddRabbitMQClient("messaging");
    builder.AddMinioClient("minio");

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    var connectionString = builder.Configuration.GetConnectionString("innoshop-users");
    if (connectionString != null &&
        !connectionString.Contains("DataSource", StringComparison.OrdinalIgnoreCase) &&
        !connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
        builder.EnrichNpgsqlDbContext<UserManagementDbContext>();
}

var app = builder.Build();
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

    var connectionString = app.Configuration.GetConnectionString("innoshop-users");
    if (connectionString != null &&
        !connectionString.Contains("DataSource", StringComparison.OrdinalIgnoreCase) &&
        !connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
    {
        dbContext.Database.Migrate();
    }

    app.UseExceptionHandler();
    app.AddInfrastructureMiddleware();

   if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}

