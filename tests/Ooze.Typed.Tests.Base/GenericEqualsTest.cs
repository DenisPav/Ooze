using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Base.Setup;
using Ooze.Typed.Tests.Base.Setup.Async;

namespace Ooze.Typed.Tests.Base;

public abstract class GenericEqualsTest<TFixture>(TFixture fixture) : GenericTest<TFixture>
    where TFixture : DbFixture
{
    #region NonAsync

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Should_Correctly_Filter_Data_By_Equal_Int_Filter(int postId)
    {
        using var scope = DbFixture.CreateServiceProvider<PostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(postId, null, null, null))
            .Apply();

        var results = await query.ToListAsync();
        Assert.Single(results);
        Assert.True(results[0].Id == postId);
    }

    [Theory]
    [InlineData("1_Sample_post_1")]
    [InlineData("5_Sample_post_5")]
    [InlineData("10_Sample_post_10")]
    [InlineData("100_Sample_post_100")]
    public async Task Should_Correctly_Filter_Data_By_Equal_String_Filter(string postName)
    {
        using var scope = DbFixture.CreateServiceProvider<PostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, postName, null, null))
            .Apply();

        var results = await query.ToListAsync();
        Assert.Single(results);
        Assert.True(results[0].Name == postName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Correctly_Filter_Data_By_Equal_Bool_Filter(bool enabled)
    {
        using var scope = DbFixture.CreateServiceProvider<PostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, enabled, null))
            .Apply();

        var results = await query.ToListAsync();
        Assert.Equal(50, results.Count);
        Assert.True(results.All(x => x.Enabled == enabled) == true);
    }

    [Theory]
    [InlineData(2022, 1, 2)]
    [InlineData(2022, 1, 3)]
    [InlineData(2022, 2, 1)]
    public async Task Should_Correctly_Filter_Data_By_Equal_DateTime_Filter(
        int year,
        int month,
        int day)
    {
        using var scope = DbFixture.CreateServiceProvider<PostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        var filterDate = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, null, filterDate))
            .Apply();

        var results = await query.ToListAsync();
        Assert.Single(results);
        Assert.True(results[0].Date.Date == filterDate);
    }

    #endregion
    
    #region Async
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public virtual async Task Should_Correctly_Filter_Data_By_Equal_Int_Filter_Async(int postId)
    {
        using var scope = DbFixture.CreateServiceProvider<AsyncPostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;
        
        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();
        
        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(postId, null, null, null))
            .ApplyAsync();
        
        var results = await query.ToListAsync();
        Assert.Single(results);
        Assert.True(results[0].Id == postId);
    }
    
    [Theory]
    [InlineData("1_Sample_post_1")]
    [InlineData("5_Sample_post_5")]
    [InlineData("10_Sample_post_10")]
    [InlineData("100_Sample_post_100")]
    public async Task Should_Correctly_Filter_Data_By_Equal_String_Filter_Async(string postName)
    {
        using var scope = DbFixture.CreateServiceProvider<AsyncPostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, postName, null, null))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.True(results.Count == 1);
        Assert.True(results[0].Name == postName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Correctly_Filter_Data_By_Equal_Bool_Filter_Async(bool enabled)
    {
        using var scope = DbFixture.CreateServiceProvider<AsyncPostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, enabled, null))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.Equal(50, results.Count);
        Assert.True(results.All(x => x.Enabled == enabled) == true);
    }

    [Theory]
    [InlineData(2022, 1, 2)]
    [InlineData(2022, 1, 3)]
    [InlineData(2022, 2, 1)]
    public async Task Should_Correctly_Filter_Data_By_Equal_DateTime_Filter_Async(
        int year,
        int month,
        int day)
    {
        using var scope = DbFixture.CreateServiceProvider<AsyncPostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        var filterDate = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        IQueryable<Post> query = context.Set<Post>();
        
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, null, filterDate))
            .ApplyAsync();
        
        var results = await query.ToListAsync();
        Assert.Single(results);
        Assert.True(results[0].Date.Date == filterDate);
    }
    
    #endregion
}