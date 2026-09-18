using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;

namespace Ooze.Typed.Tests.Base;

public class GenericDbFixture : IAsyncLifetime
{
    protected virtual IDatabaseContainer? TestContainer => throw new NotImplementedException();

    public static IServiceProvider CreateServiceProvider<TProvider>() => new DefaultServiceProviderFactory(
        new ServiceProviderOptions
        {
            ValidateScopes = false
        }).CreateServiceProvider(CreateServiceCollection<TProvider>());

    private static IServiceCollection CreateServiceCollection<TProvider>()
    {
        var services = new ServiceCollection().AddLogging();
        var oozeBuilder = services.AddOozeTyped();
        oozeBuilder.EnableAsyncResolvers();
        oozeBuilder.Add<TProvider>();

        return services;
    }

    public virtual GenericTestDbContext CreateContext() => throw new NotImplementedException();

    public async ValueTask InitializeAsync()
    {
        if (TestContainer is null)
            return;

        await TestContainer.StartAsync()
            .ConfigureAwait(false);
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        await context.Seed();
    }

    public async ValueTask DisposeAsync()
    {
        if (TestContainer is null)
            return;

        await TestContainer
            .DisposeAsync()
            .ConfigureAwait(false);
    }
}