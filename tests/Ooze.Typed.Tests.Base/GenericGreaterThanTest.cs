using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Base.Setup;
using Ooze.Typed.Tests.Base.Setup.Async;

namespace Ooze.Typed.Tests.Base;

public abstract class GenericGreaterThanTest<TFixture>(TFixture fixture) : GenericTest<TFixture>
    where TFixture : DbFixture
{
    #region NonAsync

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Should_Correctly_Filter_Data_By_Greater_Than_Int_Filter(int postId)
    {
        using var scope = DbFixture.CreateServiceProvider<PostGreaterThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(postId, null, null, null))
            .Apply();
        var expectedCount = TestDbContext.TotalCountOfFakes - postId;

        var results = await query.ToListAsync();
        Assert.True(results.Count == expectedCount);
        Assert.True(results.All(x => x.Id > postId));
    }

    [Theory]
    [InlineData("1_Sample_post")]
    [InlineData("5_Sample_post")]
    [InlineData("10_Sample_post")]
    [InlineData("100_Sample_post")]
    public async Task Should_Fail_To_Filter_Data_By_Greater_Than_String_Filter(string postName)
    {
        using var scope = DbFixture.CreateServiceProvider<PostGreaterThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        Assert.Throws<InvalidOperationException>(() => oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, postName, null, null))
            .Apply());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Fail_To_Filter_Data_By_Greater_Than_Bool_Filter(bool enabled)
    {
        using var scope = DbFixture.CreateServiceProvider<PostGreaterThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        Assert.Throws<InvalidOperationException>(() => oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, enabled, null))
            .Apply());
    }

    [Theory]
    [InlineData(2022, 1, 2)]
    [InlineData(2022, 1, 3)]
    [InlineData(2022, 2, 1)]
    public async Task Should_Correctly_Filter_Data_By_Greater_Than_DateTime_Filter(
        int year,
        int month,
        int day)
    {
        using var scope = DbFixture.CreateServiceProvider<PostGreaterThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostFilters, PostSorters>>();

        var filterDate = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, null, filterDate))
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.All(x => x.Date != filterDate));
        Assert.True(results.All(x => x.Date > filterDate));
    }

    #endregion
    
    #region Async
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Should_Correctly_Filter_Data_By_Greater_Than_Int_Filter_Async(int postId)
    {
        using var scope = DbFixture.CreateServiceProvider<AsyncPostGreaterThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(postId, null, null, null))
            .ApplyAsync();
        var expectedCount = TestDbContext.TotalCountOfFakes - postId;

        var results = await query.ToListAsync();
        Assert.True(results.Count == expectedCount);
        Assert.True(results.All(x => x.Id > postId));
    }

    [Theory]
    [InlineData("1_Sample_post")]
    [InlineData("5_Sample_post")]
    [InlineData("10_Sample_post")]
    [InlineData("100_Sample_post")]
    public async Task Should_Fail_To_Filter_Data_By_Greater_Than_String_Filter_Async(string postName)
    {
        using var scope = DbFixture.CreateServiceProvider<AsyncPostGreaterThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, postName, null, null))
            .ApplyAsync());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Fail_To_Filter_Data_By_Greater_Than_Bool_Filter_Async(bool enabled)
    {
        using var scope = DbFixture.CreateServiceProvider<AsyncPostGreaterThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, enabled, null))
            .ApplyAsync());
    }

    [Theory]
    [InlineData(2022, 1, 2)]
    [InlineData(2022, 1, 3)]
    [InlineData(2022, 2, 1)]
    public async Task Should_Correctly_Filter_Data_By_Greater_Than_DateTime_Filter_Async(
        int year,
        int month,
        int day)
    {
        using var scope = DbFixture.CreateServiceProvider<AsyncPostGreaterThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostFilters, PostSorters>>();

        var filterDate = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, null, filterDate))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.True(results.All(x => x.Date != filterDate));
        Assert.True(results.All(x => x.Date > filterDate));
    }
    
    #endregion
}