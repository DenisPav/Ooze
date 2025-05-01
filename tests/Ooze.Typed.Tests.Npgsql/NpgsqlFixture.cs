using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;
using Testcontainers.PostgreSql;

namespace Ooze.Typed.Tests.Npgsql;

public class NpgsqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15.1")
        .WithPortBinding(5432, true)
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

    public NpgsqlContext CreateContext()
    {
        var correctConnectionString = _postgreContainer.GetConnectionString();
        var sqlServerOptions = new DbContextOptionsBuilder()
            .UseNpgsql(correctConnectionString);
        return new NpgsqlContext(sqlServerOptions.Options);
    }

    public async Task InitializeAsync()
    {
        await _postgreContainer.StartAsync()
            .ConfigureAwait(false);
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();
        await context.Database.ExecuteSqlAsync($"CREATE EXTENSION fuzzystrmatch;");
    }

    public async Task DisposeAsync()
    {
        await _postgreContainer
            .DisposeAsync()
            .ConfigureAwait(false);
    }
}