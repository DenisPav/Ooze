using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;

namespace Ooze.Typed.Tests.Integration.Setup
{
    public class DbFixture<TContext>
        where TContext : DbContext
    {
        public DbFixture()
        {
            using var scope = CreateServiceProvider().CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            context.Prepare().Wait();
        }

        public IServiceCollection CreateServiceColletion()
        {
            var services = new ServiceCollection()
                .AddDbContext<TContext>(opts => opts.UseSqlite("Data Source=./database.db;"))
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
    }
}
