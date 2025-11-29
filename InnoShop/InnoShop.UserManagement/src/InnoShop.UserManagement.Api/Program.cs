using InnoShop.UserManagement.Api.Common.Interfaces;
using InnoShop.UserManagement.Api.Exceptions;
using InnoShop.UserManagement.Application;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Infrastructure;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    builder.AddRedisDistributedCache("cache");
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

    //builder.AddSqlServerDbContext<UserManagementDbContext>("innoshop-users");
    builder.AddSqlServerClient("innoshop-users");
    builder.AddRabbitMQClient("messaging");
    builder.AddMinioClient("minio");

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
        var provider = dbContext.Database.ProviderName;

        if (provider?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) == true)
        {
            dbContext.Database.Migrate();
        }
        else if (provider?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true)
        {
            dbContext.Database.EnsureCreated();
        }
    }



    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();

    app.MapControllers();

    app.UseAuthentication();
    app.UseAuthorization();

    app.AddInfrastructureMiddleware();

    app.Run();
}

