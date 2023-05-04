using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Integration.Setup;

namespace Ooze.Typed.Tests.Integration;

public class DatabaseFilterLessThanIntegrationTests : IClassFixture<DbFixture<DatabaseContext>>
{
    readonly DbFixture<DatabaseContext> _fixture;

    public DatabaseFilterLessThanIntegrationTests(DbFixture<DatabaseContext> fixture) => _fixture = fixture;

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Should_Correctly_Filter_Data_By_Less_Than_Int_Filter(int postId)
    {
        using var scope = _fixture.CreateServiceProvider<PostLessThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = _fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(postId, null, null, null))
            .Apply();
        var expectedCount = postId - 1;

        var results = await query.ToListAsync();
        Assert.True(results.Count == expectedCount);
        Assert.True(results.All(x => x.Id < postId));
    }

    [Theory]
    [InlineData("1_Sample_post")]
    [InlineData("5_Sample_post")]
    [InlineData("10_Sample_post")]
    [InlineData("100_Sample_post")]
    public async Task Should_Fail_To_Filter_Data_By_Less_Than_String_Filter(string postName)
    {
        using var scope = _fixture.CreateServiceProvider<PostLessThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = _fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        Assert.Throws<InvalidOperationException>(() => oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, postName, null, null))
            .Apply());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Fail_To_Filter_Data_By_Less_Than_Bool_Filter(bool enabled)
    {
        using var scope = _fixture.CreateServiceProvider<PostLessThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = _fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        Assert.Throws<InvalidOperationException>(() => oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, enabled, null))
            .Apply());
    }

    [Theory]
    [InlineData(2022, 1, 2)]
    [InlineData(2022, 1, 3)]
    [InlineData(2022, 2, 1)]
    public async Task Should_Correctly_Filter_Data_By_Less_Than_DateTime_Filter(
        int year,
        int month,
        int day)
    {
        using var scope = _fixture.CreateServiceProvider<PostLessThanFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = _fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters, PostSorters>>();

        var filterDate = new DateTime(year, month, day);
        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostFilters(null, null, null, filterDate))
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.All(x => x.Date != filterDate));
        Assert.True(results.All(x => x.Date < filterDate));
    }
}