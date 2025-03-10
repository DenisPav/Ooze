using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.Web;

public class SqlServerDatabaseContext : DbContext
{
    public SqlServerDatabaseContext(DbContextOptions<SqlServerDatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqliteDatabaseContext).Assembly, type => type.Namespace == "Ooze.Typed.Web.Entities.SqlServerConfigurations");
    }
}