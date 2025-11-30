using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbit = builder.AddRabbitMQ("messaging");

var cache = builder.AddRedis("cache")
    .WithDataVolume()
    .WithRedisCommander();

var postgres = builder.AddPostgres("postgres")
    .WithHostPort(5435)
    .WithPgAdmin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var usersDatabase = postgres.AddDatabase("innoshop-users");
var productsDatabase = postgres.AddDatabase("innoshop-products");

var minio = builder.AddMinioContainer("minio")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var mailpit = builder.AddMailPit("mailpit");

var productsApi = builder.AddProject<InnoShop_ProductManagement_Api>("products-api")
    .WithReference(rabbit)
    .WithReference(cache)
    .WithReference(productsDatabase)
    .WaitFor(productsDatabase);

builder.AddProject<InnoShop_UserManagement_Api>("users-api")
    .WithExternalHttpEndpoints()
    .WithReference(rabbit)
    .WithReference(minio)
    .WithReference(cache)
    .WithReference(mailpit)
    .WithReference(usersDatabase)
    .WaitFor(usersDatabase)
    .WithReference(productsApi)
    .WaitFor(productsDatabase)
    .WithEnvironment("AppUrl", "https://localhost:7152")
    .WithEnvironment("WebAppUrl", "http://localhost:5173");


builder.Build().Run();