using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;
using Ooze.Typed.Tests.MySql.OozeConfiguration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Testcontainers.MariaDb;

namespace Ooze.Typed.Tests.MySql;

public class MySqlFixture : IAsyncLifetime
{
    private readonly MariaDbContainer _sqlServerContainer = new MariaDbBuilder()
        .WithImage("mariadb:10.9")
        .WithCleanUp(true)
        .Build();

    private static IServiceCollection CreateServiceCollection<TProvider>(bool enableAsyncSupport = false)
    {
        var services = new ServiceCollection().AddLogging();
        var oozeBuilder = services.AddOozeTyped();
        if (enableAsyncSupport == true)
            oozeBuilder.EnableAsyncResolvers();
        oozeBuilder.Add<TProvider>();

        return services;
    }

    public IServiceProvider CreateServiceProvider<TProvider>(bool enableAsyncSupport = false) => new DefaultServiceProviderFactory(
        new ServiceProviderOptions
        {
            ValidateScopes = false
        }).CreateServiceProvider(CreateServiceCollection<TProvider>(enableAsyncSupport));

    public MySqlContext CreateContext()
    {
        var correctConnectionString = _sqlServerContainer.GetConnectionString();
        var serverVersion = ServerVersion.Create(new Version("10.9"), ServerType.MariaDb);
        var sqlServerOptions = new DbContextOptionsBuilder()
            .UseMySql(correctConnectionString, serverVersion);
        return new MySqlContext(sqlServerOptions.Options);
    }

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync()
            .ConfigureAwait(false);
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();
    }

    public async Task DisposeAsync()
    {
        await _sqlServerContainer
            .DisposeAsync()
            .ConfigureAwait(false);
    }
}