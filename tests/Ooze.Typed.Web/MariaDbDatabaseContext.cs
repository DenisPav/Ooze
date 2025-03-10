using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.Web;

public class MariaDbDatabaseContext : DbContext
{
    public MariaDbDatabaseContext(DbContextOptions<MariaDbDatabaseContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqliteDatabaseContext).Assembly, type => type.Namespace == "Ooze.Typed.Web.Entities.MariaDbConfigurations");
    }
}