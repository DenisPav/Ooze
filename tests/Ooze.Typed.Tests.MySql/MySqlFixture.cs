using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base;
using Ooze.Typed.Tests.MySql;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Testcontainers.MariaDb;

[assembly: AssemblyFixture(typeof(MySqlFixture))]

namespace Ooze.Typed.Tests.MySql;

public class MySqlFixture : GenericDbFixture
{
    private static readonly IDatabaseContainer DbContainer = new MariaDbBuilder()
        .WithImage("mariadb:10.9")
        .WithCleanUp(true)
        .Build();

    protected override IDatabaseContainer TestContainer => DbContainer;

    public override MySqlContext CreateContext()
    {
        var correctConnectionString = TestContainer.GetConnectionString();
        var serverVersion = ServerVersion.Create(new Version("10.9"), ServerType.MariaDb);
        var mySqlOptions = new DbContextOptionsBuilder()
            .UseMySql(correctConnectionString, serverVersion);
        return new MySqlContext(mySqlOptions.Options);
    }
}