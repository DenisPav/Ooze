using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base;
using Ooze.Typed.Tests.Npgsql;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(NpgsqlFixture))]

namespace Ooze.Typed.Tests.Npgsql;

public class NpgsqlFixture : DbFixture
{
    private static readonly IDatabaseContainer DbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15.1")
        .WithPortBinding(5432, true)
        .WithCleanUp(true)
        .Build();

    protected override IDatabaseContainer TestContainer => DbContainer;

    public override NpgsqlContext CreateContext()
    {
        var correctConnectionString = TestContainer.GetConnectionString();
        var postgreOptions = new DbContextOptionsBuilder()
            .UseNpgsql(correctConnectionString);
        return new NpgsqlContext(postgreOptions.Options);
    }
}