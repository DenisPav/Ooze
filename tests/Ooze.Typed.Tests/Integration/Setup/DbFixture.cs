using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;

namespace Ooze.Typed.Tests.Integration.Setup
{
    public class DbFixture<TContext>
        where TContext : DbContext
    {
        private IServiceCollection CreateServiceColletion()
        {
            var services = new ServiceCollection()
                .AddLogging();
            
            services.AddOozeTyped()
                .Add<PostEqualFiltersProvider>()
                .Add<PostSortersProvider>();

            return services;
        }

        public IServiceProvider CreateServiceProvider() => new DefaultServiceProviderFactory(
            new ServiceProviderOptions
            {
                ValidateScopes = true
            }).CreateServiceProvider(CreateServiceColletion());

        public DatabaseContext CreateContext()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            
            var contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(connection)
                .Options;
            var context = new DatabaseContext(contextOptions);

            context.Prepare().Wait();

            return context;
        }
    }
}
