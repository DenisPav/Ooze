using DotNet.Testcontainers.Containers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base;
using Ooze.Typed.Tests.Sqlite;

[assembly: AssemblyFixture(typeof(SqliteFixture))]

namespace Ooze.Typed.Tests.Sqlite;

public class SqliteFixture : GenericDbFixture
{
    protected override IDatabaseContainer? TestContainer => null;

    public override SqliteContext CreateContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var contextOptions = new DbContextOptionsBuilder<SqliteContext>()
            .UseSqlite(connection)
            .Options;
        var context = new SqliteContext(contextOptions);

        context.Database.EnsureCreated();
        context.Seed().Wait();

        return context;
    }
}