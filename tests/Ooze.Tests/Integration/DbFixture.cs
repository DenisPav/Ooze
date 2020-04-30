using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Ooze.AspNetCore;

namespace Ooze.Tests.Integration
{
    public class DbFixture<TContext>
        where TContext : DbContext
    {
        public DbFixture()
        {
            using var scope = CreateScope(false);
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            context.Prepare().Wait();
        }

        public IServiceCollection Services(bool usePaging) => new ServiceCollection()
            .AddDbContext<TContext>(opts => opts.UseSqlite("Data Source=./database.db;"))
            .AddLogging()
            .AddOoze(typeof(DbFixture<>).Assembly, opts =>
            {
                opts.UseSelections = true;
                opts.Paging.UsePaging = usePaging;
            });

        IServiceProvider ServiceProvider(bool usePaging) => new DefaultServiceProviderFactory(
            new ServiceProviderOptions
            {
                ValidateScopes = true
            }).CreateServiceProvider(Services(usePaging));

        public IServiceScope CreateScope(bool usePaging = false) => ServiceProvider(usePaging).CreateScope();
    }
}
