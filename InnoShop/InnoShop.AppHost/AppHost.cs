using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var rabbit = builder.AddRabbitMQ("messaging");

var sql = builder.AddSqlServer("sql")
    .WithHostPort(1433)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var minio = builder.AddMinioContainer("minio")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var mailpit = builder.AddMailPit("mailpit");

var usersDatabase = sql.AddDatabase("innoshop-users");
var productsDatabase = sql.AddDatabase("innoshop-products");

var productsApi = builder.AddProject<Projects.InnoShop_ProductManagement_Api>("products-api")
    .WithReference(rabbit)
    .WithReference(productsDatabase)
    .WaitFor(productsDatabase);

builder.AddProject<Projects.InnoShop_UserManagement_Api>("users-api")
    .WithReference(rabbit)
    .WithHttpEndpoint(5001, name: "public")
    .WithReference(usersDatabase)
    .WithReference(productsApi)
    .WaitFor(usersDatabase)
    .WithReference(minio)
    .WithReference(mailpit)
    .WaitFor(productsDatabase)
    .WithEnvironment("AppUrl", "https://localhost:7152"); ;

builder.Build().Run();
