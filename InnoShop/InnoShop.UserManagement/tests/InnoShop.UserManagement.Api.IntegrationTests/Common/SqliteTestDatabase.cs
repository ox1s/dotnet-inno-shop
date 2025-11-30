using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace InnoShop.UserManagement.Api.IntegrationTests.Common;

public class SqliteTestDatabase : IDisposable
{
    public SqliteConnection Connection { get; private set; } = null!;

    private SqliteTestDatabase(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
        Connection.Open();
    }

    public static SqliteTestDatabase CreateAndInitialize()
    {
        var db = new SqliteTestDatabase("DataSource=:memory:");
        db.InitializeDatabase();
        return db;
    }

    private void InitializeDatabase()
    {
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseSqlite(Connection)
            .Options;

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var publisher = Substitute.For<IPublisher>();
        var logger = Substitute.For<ILogger<UserManagementDbContext>>();

        using var context = new UserManagementDbContext(options, httpContextAccessor, publisher, logger);
        context.Database.EnsureCreated();

        // EnsureCreated() не применяет HasData, поэтому инициализируем роли вручную
        if (!context.Set<Role>().Any())
        {
            context.AttachRange(Role.List);
            context.SaveChanges();
        }
    }

    public void ResetDatabase()
    {
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseSqlite(Connection)
            .Options;

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var publisher = Substitute.For<IPublisher>();
        var logger = Substitute.For<ILogger<UserManagementDbContext>>();

        using var context = new UserManagementDbContext(options, httpContextAccessor, publisher, logger);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // EnsureCreated() не применяет HasData, поэтому инициализируем роли вручную
        if (!context.Set<Role>().Any())
        {
            context.AttachRange(Role.List);
            context.SaveChanges();
        }
    }

    public void Dispose()
    {
        Connection?.Dispose();
    }
}