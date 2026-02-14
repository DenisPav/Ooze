using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base;
using Ooze.Typed.Tests.SqlServer;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(SqlServerFixture))]

namespace Ooze.Typed.Tests.SqlServer;

public class SqlServerFixture : GenericDbFixture
{
    private static readonly IDatabaseContainer DbContainer = 
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .WithPortBinding(1433, true)
        .WithCleanUp(true)
        .Build();

    protected override IDatabaseContainer TestContainer => DbContainer;
    
    public override SqlServerContext CreateContext()
    {
        var correctConnectionString = TestContainer.GetConnectionString();
        var sqlServerOptions = new DbContextOptionsBuilder()
            .UseSqlServer(correctConnectionString);
        return new SqlServerContext(sqlServerOptions.Options);
    }
}