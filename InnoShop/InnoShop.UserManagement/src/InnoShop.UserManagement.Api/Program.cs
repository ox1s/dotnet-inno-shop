using InnoShop.UserManagement.Api.Exceptions;
using InnoShop.UserManagement.Application;
using InnoShop.UserManagement.Infrastructure;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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
        dbContext.Database.Migrate();
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