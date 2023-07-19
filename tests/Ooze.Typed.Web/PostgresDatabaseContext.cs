using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.Web;

public class PostgresDatabaseContext : DbContext
{
    public PostgresDatabaseContext(DbContextOptions<PostgresDatabaseContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly, type => type.Namespace == "Ooze.Typed.Web.Entities.PostgresConfigurations");
    }
}