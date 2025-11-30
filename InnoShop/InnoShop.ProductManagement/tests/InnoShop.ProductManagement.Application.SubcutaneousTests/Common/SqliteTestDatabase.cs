using InnoShop.ProductManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace InnoShop.ProductManagement.Application.SubcutaneousTests.Common;

public class SqliteTestDatabase : IDisposable
{
    private SqliteTestDatabase(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
        Connection.Open();
    }

    public SqliteConnection Connection { get; } = null!;

    public void Dispose()
    {
        Connection?.Dispose();
    }

    public static SqliteTestDatabase CreateAndInitialize()
    {
        var db = new SqliteTestDatabase("DataSource=:memory:");
        db.InitializeDatabase();
        return db;
    }

    private void InitializeDatabase()
    {
        var options = new DbContextOptionsBuilder<ProductManagementDbContext>()
            .UseSqlite(Connection)
            .Options;

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var publisher = Substitute.For<IPublisher>();
        var logger = Substitute.For<ILogger<ProductManagementDbContext>>();

        using var context = new ProductManagementDbContext(options, httpContextAccessor, publisher, logger);
        context.Database.EnsureCreated();
    }

    public void ResetDatabase()
    {
        var options = new DbContextOptionsBuilder<ProductManagementDbContext>()
            .UseSqlite(Connection)
            .Options;

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var publisher = Substitute.For<IPublisher>();
        var logger = Substitute.For<ILogger<ProductManagementDbContext>>();

        using var context = new ProductManagementDbContext(options, httpContextAccessor, publisher, logger);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}