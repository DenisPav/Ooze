using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;
using Testcontainers.MsSql;

namespace Ooze.Typed.Tests.SqlServer;

public class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServerContainer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
            .WithPortBinding(1433, true)
            .WithCleanUp(true)
            .Build();

    private static IServiceCollection CreateServiceCollection<TProvider>()
    {
        var services = new ServiceCollection().AddLogging();
        var oozeBuilder = services.AddOozeTyped();
        oozeBuilder.EnableAsyncResolvers();
        oozeBuilder.Add<TProvider>();

        return services;
    }

    public IServiceProvider CreateServiceProvider<TProvider>() => new DefaultServiceProviderFactory(
        new ServiceProviderOptions
        {
            ValidateScopes = false
        }).CreateServiceProvider(CreateServiceCollection<TProvider>());

    public SqlServerContext CreateContext()
    {
        var correctConnectionString = _sqlServerContainer.GetConnectionString();
        var sqlServerOptions = new DbContextOptionsBuilder()
            .UseSqlServer(correctConnectionString);
        return new SqlServerContext(sqlServerOptions.Options);
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