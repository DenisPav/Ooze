using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Query.Tests.Entities;
using Ooze.Typed.Query.Tests.OozeQueryConfiguration;

namespace Ooze.Typed.Query.Tests;

public class QueryIntegrationTests(SqlServerFixture fixture) : IClassFixture<SqlServerFixture>
{
    [Theory]
    [InlineData(10)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task Id_Should_Update_Query_And_Return_Correct_Query(int postId)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.FilterWithQueryLanguage(query, $"{nameof(Post.Id)} == '{postId}'");

        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(1, filteredItemsCount);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task Name_Should_Update_Query_And_Return_Correct_Query(int postId)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.FilterWithQueryLanguage(query, $"{nameof(Post.Name)} == '{postId}_Sample_post_{postId}'");

        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(1, filteredItemsCount);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Enabled_Should_Update_Query_And_Return_Correct_Query(bool enabled)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.FilterWithQueryLanguage(query, $"{nameof(Post.Enabled)} == '{enabled}'");

        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(50, filteredItemsCount);
    }

    [Theory]
    [InlineData(2022, 1, 2, 20, 20, 22)]
    [InlineData(2022, 1, 3, 20, 20, 22)]
    public async Task Date_Should_Update_Query_And_Return_Correct_Query(
        int year,
        int month,
        int day,
        int hour,
        int minute,
        int second)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();

        query = resolver.FilterWithQueryLanguage(query, $"Date == '{month}.{day}.{year}. {hour}:{minute}:{second}'");

        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(1, filteredItemsCount);
    }
    
    [Theory]
    [InlineData(2022, 1, 2)]
    [InlineData(2022, 1, 3)]
    public async Task DateOnly_Should_Update_Query_And_Return_Correct_Query(
        int year,
        int month,
        int day)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();

        query = resolver.FilterWithQueryLanguage(query, $"{nameof(Post.OnlyDate)} == '{month}.{day}.{year}.'");

        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(1, filteredItemsCount);
    }
}