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
            using var scope = CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            context.Prepare().Wait();
        }

        public IServiceCollection Services = new ServiceCollection()
            .AddDbContext<TContext>(opts => opts.UseSqlite("Data Source=./database.db;"))
            .AddOoze(typeof(DbFixture<>).Assembly);

        IServiceProvider ServiceProvider => new DefaultServiceProviderFactory(
            new ServiceProviderOptions
            {
                ValidateScopes = true
            }).CreateServiceProvider(Services);

        public IServiceScope CreateScope() => ServiceProvider.CreateScope();
    }
}
