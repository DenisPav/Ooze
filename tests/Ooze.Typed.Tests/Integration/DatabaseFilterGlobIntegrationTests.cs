using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Integration.Setup;

namespace Ooze.Typed.Tests.Integration;

public class DatabaseFilterGlobIntegrationTests(DbFixture<DatabaseContext> fixture)
    : IClassFixture<DbFixture<DatabaseContext>>
{
    [Fact]
    public async Task Should_Correctly_Filter_Data_By_Glob_Filter()
    {
        using var scope = fixture.CreateServiceProvider<PostGlobFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostGlobFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostGlobFilters("*Sample*post"))
            .Apply();

        var results = await query.ToListAsync();
        Assert.True(results.Count == DatabaseContext.TotalCountOfFakes);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Should_Correctly_Filter_Data_By_Glob_Int_Filter(int postId)
    {
        using var scope = fixture.CreateServiceProvider<PostGlobFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostGlobFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostGlobFilters($"{postId}_Sample*post"))
            .Apply();

        var results = await query.ToListAsync();
        Assert.Single(results);
        Assert.True(results.All(x => x.Id == postId));
    }
}