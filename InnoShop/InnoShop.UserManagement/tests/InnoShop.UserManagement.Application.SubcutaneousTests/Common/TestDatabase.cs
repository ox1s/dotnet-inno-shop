using InnoShop.UserManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Common;

public class SqliteTestDatabase : IDisposable
{
    public SqliteConnection Connection { get; }

    public static SqliteTestDatabase CreateAndInitialize()
    {
        var testDatabase = new SqliteTestDatabase("DataSource=:memory:");
        testDatabase.InitializeDatabase();
        return testDatabase;
    }

    public void InitializeDatabase()
    {
        Connection.Open();
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseSqlite(Connection)
            .Options;

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var publisher = Substitute.For<IPublisher>();
        var logger = Substitute.For<ILogger<UserManagementDbContext>>();

        var context = new UserManagementDbContext(options, httpContextAccessor, publisher, logger);
        context.Database.EnsureCreated();
    }

    public void ResetDatabase()
    {
        Connection.Close();
        InitializeDatabase();
    }

    private SqliteTestDatabase(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
    }

    public void Dispose()
    {
        Connection.Close();
    }
}
