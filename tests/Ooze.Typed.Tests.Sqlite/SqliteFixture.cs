using DotNet.Testcontainers.Containers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base;
using Ooze.Typed.Tests.Sqlite.Setup;

namespace Ooze.Typed.Tests.Sqlite;

public class SqliteFixture : DbFixture
{
    protected override IDatabaseContainer? TestContainer => null;

    public override DatabaseContext CreateContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;
        var context = new DatabaseContext(contextOptions);

        context.Database.EnsureCreated();
        context.Seed().Wait();

        return context;
    }
}