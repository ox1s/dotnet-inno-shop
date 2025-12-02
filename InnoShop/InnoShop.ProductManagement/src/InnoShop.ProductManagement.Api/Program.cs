using System;
using InnoShop.ProductManagement.Api.Exceptions;
using InnoShop.ProductManagement.Api.Services;
using InnoShop.ProductManagement.Application;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Infrastructure;
using InnoShop.ProductManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    builder.AddServiceDefaults();

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


    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    var connectionString = builder.Configuration.GetConnectionString("innoshop-products");
    if (connectionString != null &&
        !connectionString.Contains("DataSource", StringComparison.OrdinalIgnoreCase) &&
        !connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
        builder.EnrichNpgsqlDbContext<ProductManagementDbContext>();
}

var app = builder.Build();
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();

    var connectionString = app.Configuration.GetConnectionString("innoshop-products");
    if (connectionString != null &&
        !connectionString.Contains("DataSource", StringComparison.OrdinalIgnoreCase) &&
        !connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
    {
        dbContext.Database.Migrate();
    }
    else if (connectionString != null &&
             (connectionString.Contains("DataSource", StringComparison.OrdinalIgnoreCase) ||
              connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase)))
    {
        dbContext.Database.EnsureCreated();
    }

    app.MapDefaultEndpoints();

    app.UseExceptionHandler();

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