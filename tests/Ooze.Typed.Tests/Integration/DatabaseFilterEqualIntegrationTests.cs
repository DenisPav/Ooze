using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Integration.Setup;

namespace Ooze.Typed.Tests.Integration;

public class DatabaseFilterEqualIntegrationTests : IClassFixture<DbFixture<DatabaseContext>>
{
    readonly DbFixture<DatabaseContext> _fixture;

    public DatabaseFilterEqualIntegrationTests(DbFixture<DatabaseContext> fixture) => _fixture = fixture;

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Should_Correctly_Filter_Data_By_Equal_Int_Filter(int postId)
    {
        using var scope = _fixture.CreateServiceProvider<PostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = _fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(postId, null, null, null))
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.Count == 1);
        Assert.True(results[0].Id == postId);
    }

    [Theory]
    [InlineData("1_Sample_post")]
    [InlineData("5_Sample_post")]
    [InlineData("10_Sample_post")]
    [InlineData("100_Sample_post")]
    public async Task Should_Correctly_Filter_Data_By_Equal_String_Filter(string postName)
    {
        using var scope = _fixture.CreateServiceProvider<PostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = _fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, postName, null, null))
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.Count == 1);
        Assert.True(results[0].Name == postName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Correctly_Filter_Data_By_Equal_Bool_Filter(bool enabled)
    {
        using var scope = _fixture.CreateServiceProvider<PostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = _fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, enabled, null))
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.Count == 50);
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
        using var scope = _fixture.CreateServiceProvider<PostEqualFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = _fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters, PostSorters>>();

        var filterDate = new DateTime(year, month, day);
        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, null, filterDate))
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.Count == 1);
        Assert.True(results[0].Date == filterDate);
    }
}