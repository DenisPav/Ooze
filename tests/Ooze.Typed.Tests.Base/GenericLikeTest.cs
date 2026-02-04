using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Base.Setup;
using Ooze.Typed.Tests.Base.Setup.Async;

namespace Ooze.Typed.Tests.Base;

public abstract class GenericLikeTest<TFixture>(TFixture fixture) : GenericTest<TFixture>
    where TFixture : DbFixture
{
    #region NonAsync

    //TODO: add they are missing

    #endregion
    
    #region Async
    
    [Fact]
    public async Task Should_Correctly_Filter_Data_By_Like_Filter_Async()
    {
        using var scope = DbFixture.CreateServiceProvider<PostLikeFiltersProvider>().CreateScope();
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
        Assert.True(results.Count == TestDbContext.TotalCountOfFakes);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Should_Correctly_Filter_Data_By_Like_Int_Filter_Async(int postId)
    {
        using var scope = DbFixture.CreateServiceProvider<PostLikeFiltersProvider>().CreateScope();
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
    
    #endregion
}