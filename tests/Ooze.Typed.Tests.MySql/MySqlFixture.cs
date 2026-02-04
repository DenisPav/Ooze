using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Testcontainers.MariaDb;

namespace Ooze.Typed.Tests.MySql;

public class MySqlFixture : DbFixture
{
    protected override IDatabaseContainer TestContainer { get; } = new MariaDbBuilder()
        .WithImage("mariadb:10.9")
        .WithCleanUp(true)
        .Build();

    public override MySqlContext CreateContext()
    {
        var correctConnectionString = TestContainer.GetConnectionString();
        var serverVersion = ServerVersion.Create(new Version("10.9"), ServerType.MariaDb);
        var mySqlOptions = new DbContextOptionsBuilder()
            .UseMySql(correctConnectionString, serverVersion);
        return new MySqlContext(mySqlOptions.Options);
    }
}