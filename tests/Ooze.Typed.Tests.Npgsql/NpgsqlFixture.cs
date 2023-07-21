﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;
using Ooze.Typed.Tests.Npgsql.OozeConfiguration;
using Testcontainers.PostgreSql;

namespace Ooze.Typed.Tests.Npgsql;

public class NpgsqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _sqlServerContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15.1")
        .WithPortBinding(5432, true)
        .WithCleanUp(true)
        .Build();

    private static IServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection().AddLogging();
        services.AddOozeTyped()
            .Add<PostFiltersProvider>()
            .Add<PostSortersProvider>();

        return services;
    }

    public readonly IServiceProvider ServiceProvider = new DefaultServiceProviderFactory(
        new ServiceProviderOptions
        {
            ValidateScopes = false
        }).CreateServiceProvider(CreateServiceCollection());

    public NpgsqlContext CreateContext()
    {
        var correctConnectionString = _sqlServerContainer.GetConnectionString();
        var sqlServerOptions = new DbContextOptionsBuilder()
            .UseNpgsql(correctConnectionString);
        return new NpgsqlContext(sqlServerOptions.Options);
    }

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync()
            .ConfigureAwait(false);
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();
        await context.Database.ExecuteSqlAsync($"CREATE EXTENSION fuzzystrmatch;");
    }

    public async Task DisposeAsync()
    {
        await _sqlServerContainer
            .DisposeAsync()
            .ConfigureAwait(false);
    }
}