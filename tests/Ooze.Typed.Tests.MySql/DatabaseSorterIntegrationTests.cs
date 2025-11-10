using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Sorters;
using Ooze.Typed.Tests.MySql.Setup;

namespace Ooze.Typed.Tests.MySql;

public class DatabaseSorterEqualIntegrationTests(MySqlFixture fixture)
    : IClassFixture<MySqlFixture>
{
    [Fact]
    public async Task Should_Correctly_Sort_Data_Descending_By_Single_Field()
    {
        using var scope = fixture.CreateServiceProvider<PostSortersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        var sorters = new[]
        {
            new PostSorters(SortDirection.Descending, null, null)
        };
        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Sort(sorters)
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.Count == MySqlContext.TotalCountOfFakes);
        Assert.True(results[0].Id == 100);
        Assert.True(results[99].Id == 1);
    }

    [Fact]
    public async Task Should_Correctly_Sort_Data_Ascending_By_Single_Field()
    {
        using var scope = fixture.CreateServiceProvider<PostSortersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        var sorters = new[]
        {
            new PostSorters(SortDirection.Ascending, null, null)
        };
        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Sort(sorters)
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.Count == MySqlContext.TotalCountOfFakes);
        Assert.True(results[0].Id == 1);
        Assert.True(results[99].Id == 100);
    }

    [Fact]
    public async Task Should_Correctly_Sort_Data_Ascending_By_Multple_Fields()
    {
        using var scope = fixture.CreateServiceProvider<PostSortersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        var sorters = new[]
        {
            new PostSorters(SortDirection.Ascending, null, null),
            new PostSorters(null, null, SortDirection.Ascending)
        };
        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Sort(sorters)
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.Count == MySqlContext.TotalCountOfFakes);
        Assert.True(results[0].Id == 1);
        Assert.True(results[1].Id == 2);
    }

    [Fact]
    public async Task Should_Not_Sort_Data_If_Sort_Not_Provided()
    {
        using var scope = fixture.CreateServiceProvider<PostSortersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        var sorters = Enumerable.Empty<PostSorters>();
        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Sort(sorters)
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.Count == MySqlContext.TotalCountOfFakes);
        Assert.True(results[0].Id == 1);
        Assert.True(results[99].Id == 100);
    }
}
