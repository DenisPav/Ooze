using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Sorters;
using Ooze.Typed.Tests.MySql.OozeConfiguration.Async;

namespace Ooze.Typed.Tests.MySql;

public class MySqlAsyncIntegrationTests(MySqlFixture fixture) : IClassFixture<MySqlFixture>
{
    [Fact]
    public async Task DateDiffDay_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostAsyncFiltersProvider>(true)
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = await resolver.FilterAsync(query,
            new PostFilters(DateDiffDayFilter: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(DAY,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffMonth_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostAsyncFiltersProvider>(true)
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = await resolver.FilterAsync(query,
            new PostFilters(DateDiffMonthFilter: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(MONTH,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffYear_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostAsyncFiltersProvider>(true)
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = await resolver.FilterAsync(query,
            new PostFilters(DateDiffYearFilter: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(YEAR,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffHour_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostAsyncFiltersProvider>(true)
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = await resolver.FilterAsync(query,
            new PostFilters(DateDiffHourFilter: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(HOUR,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffMinute_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostAsyncFiltersProvider>(true)
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = await resolver.FilterAsync(query,
            new PostFilters(DateDiffMinuteFilter: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(MINUTE,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffSecond_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostAsyncFiltersProvider>(true)
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = await resolver.FilterAsync(query,
            new PostFilters(DateDiffSecondFilter: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(SECOND,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffMicrosecond_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostAsyncFiltersProvider>(true)
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = await resolver.FilterAsync(query,
            new PostFilters(DateDiffMicrosecondFilter: new DateTime(2022, 2, 2, 20, 20, 22)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(MICROSECOND,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    }

    [Fact]
    public async Task Id_Sorter_Should_Update_Query_And_Return_Correctly_Ordered_Data()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostAsyncSortersProvider>(true)
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        var defaultIds = await query.Select(x => x.Id)
            .ToListAsync();

        query = await resolver.SortAsync(query,
            new[] { new PostSorters(Id: SortDirection.Descending) });
        var sortedIds = await query.Select(x => x.Id)
            .ToListAsync();

        Assert.False(defaultIds.SequenceEqual(sortedIds));
        Assert.False(defaultIds.Except(sortedIds).Any());
        Assert.Equal(100, defaultIds.Intersect(sortedIds).Count());

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ORDER BY", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    }
}