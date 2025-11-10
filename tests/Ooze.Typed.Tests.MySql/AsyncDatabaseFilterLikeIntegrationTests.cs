using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.EntityFrameworkCore.Extensions;
using Ooze.Typed.Filters;
using Ooze.Typed.Filters.Async;
using Ooze.Typed.Tests.MySql.Setup;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterLikeIntegrationTests(MySqlFixture fixture)
    : IClassFixture<MySqlFixture>
{
    [Fact]
    public async Task Should_Correctly_Filter_Data_By_Like_Filter()
    {
        using var scope = fixture.CreateServiceProvider<PostLikeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostLikeFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostLikeFilters("%Sample_post%"))
            .ApplyAsync();

        var queryString = query.ToQueryString();
        var results = await query.ToListAsync();
        Assert.True(queryString.Contains("LIKE", StringComparison.InvariantCultureIgnoreCase));
        Assert.True(results.Count == MySqlContext.TotalCountOfFakes);
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
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostLikeFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostLikeFilters($"{postId}_Sample%"))
            .ApplyAsync();

        var queryString = query.ToQueryString();
        var results = await query.ToListAsync();
        Assert.Single(results);
        Assert.True(queryString.Contains("LIKE", StringComparison.InvariantCultureIgnoreCase));
        Assert.True(results.All(x => x.Id == postId));
    }
}

public record PostLikeFilters(string LikeExpr);

public class PostLikeFiltersProvider : IFilterProvider<Post, PostLikeFilters>,
    IAsyncFilterProvider<Post, PostLikeFilters>
{
    public IEnumerable<FilterDefinition<Post, PostLikeFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostLikeFilters>()
            .Like(post => post.Name, filter => filter.LikeExpr)
            .Build();

    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostLikeFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostLikeFilters>()
            .Like(post => post.Name, filter => filter.LikeExpr)
            .Build());
}