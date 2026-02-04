using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base;
using Testcontainers.PostgreSql;

namespace Ooze.Typed.Tests.Npgsql;

public class NpgsqlFixture : DbFixture
{
    protected override IDatabaseContainer TestContainer { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:15.1")
        .WithPortBinding(5432, true)
        .WithCleanUp(true)
        .Build();

    public override NpgsqlContext CreateContext()
    {
        var correctConnectionString = TestContainer.GetConnectionString();
        var postgreOptions = new DbContextOptionsBuilder()
            .UseNpgsql(correctConnectionString);
        return new NpgsqlContext(postgreOptions.Options);
    }
}