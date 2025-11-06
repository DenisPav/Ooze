using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Sqlite.Setup;

namespace Ooze.Typed.Tests.Sqlite;

public class DatabaseFilterLikeIntegrationTests(DbFixture<DatabaseContext> fixture)
    : IClassFixture<DbFixture<DatabaseContext>>
{
    [Fact]
    public async Task Should_Correctly_Filter_Data_By_Like_Filter()
    {
        using var scope = fixture.CreateServiceProvider<PostLikeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostLikeFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostLikeFilters("%Sample_post%"))
            .Apply();

        var queryString = query.ToQueryString();
        var results = await query.ToListAsync();
        Assert.True(queryString.Contains("LIKE", StringComparison.InvariantCultureIgnoreCase));
        Assert.True(results.Count == DatabaseContext.TotalCountOfFakes);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Should_Correctly_Filter_Data_By_Like_Int_Filter(int postId)
    {
        using var scope = fixture.CreateServiceProvider<PostLikeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostLikeFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostLikeFilters($"{postId}_Sample%"))
            .Apply();

        var queryString = query.ToQueryString();
        var results = await query.ToListAsync();
        Assert.Single(results);
        Assert.True(queryString.Contains("LIKE", StringComparison.InvariantCultureIgnoreCase));
        Assert.True(results.All(x => x.Id == postId));
    }
}