using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.Web;

public class SqliteDatabaseContext(DbContextOptions<SqliteDatabaseContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqliteDatabaseContext).Assembly,
            //TODO: move this to a sub folder so that each context has related entity configurations
            //TODO: maybe using a single set of configurations is possible across multiple db context implementations
            type => type.Namespace == "Ooze.Typed.Web.Entities");
    }
}