using InnoShop.ProductManagement.Api.Services;
using InnoShop.ProductManagement.Application;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Infrastructure;
using InnoShop.ProductManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen();

    builder.Services.AddProblemDetails();
    builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

    builder.AddRabbitMQClient("messaging");


    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();
        var provider = dbContext.Database.ProviderName;
        if (provider?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) == true)
            dbContext.Database.Migrate();
        else if (provider?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true)
            dbContext.Database.EnsureCreated();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.MapControllers();

    app.Run();
}