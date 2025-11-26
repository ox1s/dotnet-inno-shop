using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

var rabbit = builder.AddRabbitMQ("messaging");

var postgres = builder.AddPostgres("postgres")
    .WithHostPort(5432)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var usersDatabase = postgres.AddDatabase("innoshop-users");
var productsDatabase = postgres.AddDatabase("innoshop-products");

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
    .WaitFor(productsDatabase);

builder.Build().Run();
