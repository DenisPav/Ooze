using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Base.Setup;
using Ooze.Typed.Tests.Base.Setup.Async;

namespace Ooze.Typed.Tests.Base;

public abstract class GenericInTest<TFixture>(TFixture fixture) : GenericTest<TFixture>
    where TFixture : GenericDbFixture
{
    #region NonAsync

    [Theory]
    [InlineData(1, 2, 3, 4)]
    [InlineData(5, 6, 7, 8)]
    [InlineData(10, 11, 12, 13)]
    [InlineData(100, 101, 102, 103)]
    public async Task Should_Correctly_Filter_Data_By_In_Int_Filter(params int[] postIds)
    {
        var castedIds = postIds.Select(x => (long)x);
        var validIds = castedIds.Where(x => x <= GenericTestDbContext.TotalCountOfFakes);
        using var scope = GenericDbFixture.CreateServiceProvider<PostInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(castedIds, null, null))
            .Apply();


        var results = await query.ToListAsync();
        Assert.True(results.Count == validIds.Count());
        Assert.True(results.All(x => validIds.Contains(x.Id)));
    }

    [Theory]
    [InlineData("1_Sample_post_1", "2_Sample_post_2", "3_Sample_post_3", "4_Sample_post_4")]
    [InlineData("5_Sample_post_5", "6_Sample_post_6", "7_Sample_post_7", "8_Sample_post_8")]
    [InlineData("10_Sample_post_10", "11_Sample_post_11", "12_Sample_post_12", "13_Sample_post_13")]
    [InlineData("100_Sample_post_100", "101_Sample_post_101", "102_Sample_post_102", "103")]
    public async Task Should_Correctly_Filter_Data_By_In_String_Filter(params string[] postNames)
    {
        using var scope = GenericDbFixture.CreateServiceProvider<PostInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(null, postNames, null))
            .Apply();


        var results = await query.ToListAsync();
        Assert.True(results.Count <= postNames.Length);
        Assert.True(results.All(x => postNames.Contains(x.Name)));
    }

    [Theory]
    [ClassData(typeof(DateData))]
    public async Task Should_Correctly_Filter_Data_By_In_DateTime_Filter(params DateTime[] dates)
    {
        using var scope = GenericDbFixture.CreateServiceProvider<PostInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(null, null, dates))
            .Apply();


        var results = await query.ToListAsync();
        Assert.True(results.Count <= dates.Length);
        Assert.True(results.All(x => dates.Contains(x.Date)));
    }

    #endregion
    
    #region Async
    
    [Theory]
    [InlineData(1, 2, 3, 4)]
    [InlineData(5, 6, 7, 8)]
    [InlineData(10, 11, 12, 13)]
    [InlineData(100, 101, 102, 103)]
    public async Task Should_Correctly_Filter_Data_By_In_Int_Filter_Async(params int[] postIds)
    {
        var castedIds = postIds.Select(x => (long)x);
        var validIds = castedIds.Where(x => x <= GenericTestDbContext.TotalCountOfFakes);
        using var scope = GenericDbFixture.CreateServiceProvider<AsyncPostInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(castedIds, null, null))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.True(results.Count == validIds.Count());
        Assert.True(results.All(x => validIds.Contains(x.Id)));
    }

    [Theory]
    [InlineData("1_Sample_post", "2_Sample_post", "3_Sample_post", "4_Sample_post")]
    [InlineData("5_Sample_post", "6_Sample_post", "7_Sample_post", "8_Sample_post")]
    [InlineData("10_Sample_post", "11_Sample_post", "12_Sample_post", "13_Sample_post")]
    [InlineData("100_Sample_post", "101_Sample_post", "102_Sample_post", "103")]
    public async Task Should_Correctly_Filter_Data_By_In_String_Filter_Async(params string[] postNames)
    {
        using var scope = GenericDbFixture.CreateServiceProvider<AsyncPostInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(null, postNames, null))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.True(results.Count <= postNames.Length);
        Assert.True(results.All(x => postNames.Contains(x.Name)));
    }
    
    [Theory]
    [ClassData(typeof(DateData))]
    public async Task Should_Correctly_Filter_Data_By_In_DateTime_Filter_Async(params DateTime[] dates)
    {
        using var scope = GenericDbFixture.CreateServiceProvider<AsyncPostInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;
    
        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostInFilters, PostSorters>>();
    
        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(null, null, dates))
            .ApplyAsync();
    
        var results = await query.ToListAsync();
        Assert.True(results.Count <= dates.Length);
        Assert.True(results.All(x => dates.Contains(x.Date.Date)));
    }
    
    #endregion
}