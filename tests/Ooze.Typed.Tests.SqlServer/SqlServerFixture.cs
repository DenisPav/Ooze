using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base;
using Testcontainers.MsSql;

namespace Ooze.Typed.Tests.SqlServer;

public class SqlServerFixture : DbFixture
{
    protected override IDatabaseContainer TestContainer { get; } =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
            .WithPortBinding(1433, true)
            .WithCleanUp(true)
            .Build();
    
    public override SqlServerContext CreateContext()
    {
        var correctConnectionString = TestContainer.GetConnectionString();
        var sqlServerOptions = new DbContextOptionsBuilder()
            .UseSqlServer(correctConnectionString);
        return new SqlServerContext(sqlServerOptions.Options);
    }
}