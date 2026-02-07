using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;
using Ooze.Typed.Tests.Base.Setup;

namespace Ooze.Typed.Tests.Base;

//move to Ooze.Typed.Tests then reference that project from other tests projects
//move tests into Collection fixtures based on this https://github.com/xunit/xunit/discussions/2834#discussioncomment-7758196
//in order to reduce number of containers starting up for each case
public abstract class GenericTest<TFixture> 
    where TFixture : DbFixture;


public class DbFixture : IAsyncLifetime
{
    protected virtual IDatabaseContainer? TestContainer { get; }
    
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

    public virtual TestDbContext CreateContext() => throw new NotImplementedException();

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

public abstract class TestDbContext(DbContextOptions options) 
    : DbContext(options)
{
    public const int TotalCountOfFakes = 100;

    public virtual async Task Seed()
    {
        var date = new DateTime(2022, 1, 1, 20, 20, 22);
        var posts = Enumerable.Range(1, TotalCountOfFakes)
            .Select(id => new Post
            {
                Id = 0,
                Enabled = id % 2 == 0,
                Name = $"{id}_Sample_post_{id}",
                Date = date.AddDays(id).ToUniversalTime(),
                Comments =
                [
                    new Comment
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(id).ToUniversalTime(),
                        Text = $"Sample comment {id}",
                        User = new User
                        {
                            Id = 0,
                            Email = $"sample_{id}@email.com"
                        }
                    }
                ]
            });


        Set<Post>().AddRange(posts);
        await SaveChangesAsync();
    }
}