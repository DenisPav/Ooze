using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;
using Ooze.Typed.Query.Extensions;

namespace Ooze.Typed.Query.Tests.Integration.Setup
{
    public class DbFixture<TContext>
        where TContext : DbContext
    {
        private IServiceCollection CreateServiceColletion<TProvider1>()
        {
            var services = new ServiceCollection()
                .AddLogging();

            services.AddOozeTyped()
                .AddQueryHandler()
                .AddQueryFilter<TProvider1>();

            return services;
        }

        public IServiceProvider CreateServiceProvider<TProvider1>() => new DefaultServiceProviderFactory(
            new ServiceProviderOptions
            {
                ValidateScopes = true
            }).CreateServiceProvider(CreateServiceColletion<TProvider1>());

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
