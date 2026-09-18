using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.Web;

public class SqliteDatabaseContext(DbContextOptions<SqliteDatabaseContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqliteDatabaseContext).Assembly,
            type => type.Namespace == "Ooze.Typed.Web.Entities");
    }
}