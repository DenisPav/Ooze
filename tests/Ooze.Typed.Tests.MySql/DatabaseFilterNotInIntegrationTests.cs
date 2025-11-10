using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.MySql.Setup;

namespace Ooze.Typed.Tests.MySql;

public class DatabaseFilterNotInThanIntegrationTests(MySqlFixture fixture)
    : IClassFixture<MySqlFixture>
{
    [Theory]
    [InlineData(1, 2, 3, 4)]
    [InlineData(5, 6, 7, 8)]
    [InlineData(10, 11, 12, 13)]
    [InlineData(100, 101, 102, 103)]
    public async Task Should_Correctly_Filter_Data_By_Not_In_Int_Filter(params int[] postIds)
    {
        var castedIds = postIds.Select(x => (long)x);
        var validIds = castedIds.Where(x => x <= MySqlContext.TotalCountOfFakes);
        using var scope = fixture.CreateServiceProvider<PostNotInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(castedIds, null, null))
            .Apply();


        var results = await query.ToListAsync();
        Assert.True(results.All(x => validIds.Contains(x.Id) == false));
    }

    [Theory]
    [InlineData("1_Sample_post_1", "2_Sample_post_2", "3_Sample_post_3", "4_Sample_post_4")]
    [InlineData("5_Sample_post_5", "6_Sample_post_6", "7_Sample_post_7", "8_Sample_post_8")]
    [InlineData("10_Sample_post_10", "11_Sample_post_11", "12_Sample_post_12", "13_Sample_post_13")]
    [InlineData("100_Sample_post_100", "101_Sample_post_101", "102_Sample_post_102", "103")]
    public async Task Should_Correctly_Filter_Data_By_Not_In_String_Filter(params string[] postNames)
    {
        using var scope = fixture.CreateServiceProvider<PostNotInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(null, postNames, null))
            .Apply();


        var results = await query.ToListAsync();
        Assert.True(results.All(x => postNames.Contains(x.Name) == false));
    }

    [Theory]
    [ClassData(typeof(DateData))]
    public async Task Should_Correctly_Filter_Data_By_Not_In_DateTime_Filter(params DateTime[] dates)
    {
        using var scope = fixture.CreateServiceProvider<PostNotInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(null, null, dates))
            .Apply();


        var results = await query.ToListAsync();
        Assert.True(results.All(x => dates.Contains(x.Date) == false));
    }
}