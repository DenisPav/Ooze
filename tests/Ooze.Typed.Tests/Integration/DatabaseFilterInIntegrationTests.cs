using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Integration.Setup;

namespace Ooze.Typed.Tests.Integration;

public class DatabaseFilterInThanIntegrationTests(DbFixture<DatabaseContext> fixture)
    : IClassFixture<DbFixture<DatabaseContext>>
{
    [Theory]
    [InlineData(1, 2, 3, 4)]
    [InlineData(5, 6, 7, 8)]
    [InlineData(10, 11, 12, 13)]
    [InlineData(100, 101, 102, 103)]
    public async Task Should_Correctly_Filter_Data_By_In_Int_Filter(params int[] postIds)
    {
        var castedIds = postIds.Select(x => (long)x);
        var validIds = castedIds.Where(x => x <= DatabaseContext.TotalCountOfFakes);
        using var scope = fixture.CreateServiceProvider<PostInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(castedIds, null, null))
            .Apply();


        var results = await query.ToListAsync();
        Assert.True(results.Count == validIds.Count());
        Assert.True(results.All(x => validIds.Contains(x.Id)));
    }

    [Theory]
    [InlineData("1_Sample_post", "2_Sample_post", "3_Sample_post", "4_Sample_post")]
    [InlineData("5_Sample_post", "6_Sample_post", "7_Sample_post", "8_Sample_post")]
    [InlineData("10_Sample_post", "11_Sample_post", "12_Sample_post", "13_Sample_post")]
    [InlineData("100_Sample_post", "101_Sample_post", "102_Sample_post", "103")]
    public async Task Should_Correctly_Filter_Data_By_In_String_Filter(params string[] postNames)
    {
        using var scope = fixture.CreateServiceProvider<PostInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
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
        using var scope = fixture.CreateServiceProvider<PostInFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostInFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostInFilters(null, null, dates))
            .Apply();


        var results = await query.ToListAsync();
        Assert.True(results.Count <= dates.Length);
        Assert.True(results.All(x => dates.Contains(x.Date)));
    }
}

class DateData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var dateChunks = Enumerable.Range(1, 101)
            .Select(x => new DateTime(2022, 1, 1).AddDays(x))
            .Chunk(25);

        foreach (var chunk in dateChunks)
        {
            yield return chunk.Cast<object>().ToArray();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}