using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Sqlite.Setup;
using Ooze.Typed.Tests.Sqlite.Setup.Async;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseFilterNotEqualIntegrationTests(DbFixture<DatabaseContext> fixture)
    : IClassFixture<DbFixture<DatabaseContext>>
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Should_Correctly_Filter_Data_By_Not_Equal_Int_Filter(int postId)
    {
        using var scope = fixture.CreateServiceProvider<AsyncPostNotEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(postId, null, null, null))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.True(results.Count == 99);
        Assert.True(results.All(x => x.Id != postId));
    }

    [Theory]
    [InlineData("1_Sample_post")]
    [InlineData("5_Sample_post")]
    [InlineData("10_Sample_post")]
    [InlineData("100_Sample_post")]
    public async Task Should_Correctly_Filter_Data_By_Not_Equal_String_Filter(string postName)
    {
        using var scope = fixture.CreateServiceProvider<AsyncPostNotEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, postName, null, null))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.True(results.Count == 99);
        Assert.True(results.All(x => x.Name != postName));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Correctly_Filter_Data_By_Not_Equal_Bool_Filter(bool enabled)
    {
        using var scope = fixture.CreateServiceProvider<AsyncPostNotEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, enabled, null))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.True(results.Count == 50);
        Assert.True(results.All(x => x.Enabled != enabled));
    }

    [Theory]
    [InlineData(2022, 1, 2)]
    [InlineData(2022, 1, 3)]
    [InlineData(2022, 2, 1)]
    public async Task Should_Correctly_Filter_Data_By_Not_Equal_DateTime_Filter(
        int year,
        int month,
        int day)
    {
        using var scope = fixture.CreateServiceProvider<AsyncPostNotEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        var filterDate = new DateTime(year, month, day);
        IQueryable<Post> query = context.Posts;
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, null, filterDate))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.True(results.Count == 99);
        Assert.True(results.All(x => x.Date != filterDate));
    }
}