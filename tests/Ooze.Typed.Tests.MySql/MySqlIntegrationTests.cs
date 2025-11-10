using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Sorters;
using Ooze.Typed.Tests.MySql.Setup;

namespace Ooze.Typed.Tests.MySql;

public class MySqlIntegrationTests(MySqlFixture fixture) : IClassFixture<MySqlFixture>
{
    [Fact]
    public async Task DateDiffDay_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostDateFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostDateFilters(DateDiffDayFilter: new DateTime(2022, 5, 20)));

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

        var resolver = fixture.CreateServiceProvider<PostDateFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostDateFilters(DateDiffMonthFilter: new DateTime(2022, 5, 20)));

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

        var resolver = fixture.CreateServiceProvider<PostDateFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostDateFilters(DateDiffYearFilter: new DateTime(2022, 5, 20)));

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

        var resolver = fixture.CreateServiceProvider<PostDateFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostDateFilters(DateDiffHourFilter: new DateTime(2022, 5, 20)));

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

        var resolver = fixture.CreateServiceProvider<PostDateFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostDateFilters(DateDiffMinuteFilter: new DateTime(2022, 5, 20)));

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

        var resolver = fixture.CreateServiceProvider<PostDateFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostDateFilters(DateDiffSecondFilter: new DateTime(2022, 5, 20)));

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

        var resolver = fixture.CreateServiceProvider<PostDateFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostDateFilters(DateDiffMicrosecondFilter: new DateTime(2022, 2, 2, 20, 20, 22)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(MICROSECOND,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    }

    [Fact]
    public async Task Id_Sorter_Should_Update_Query_And_Return_Correctly_Ordered_Data()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostSortersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        var defaultIds = await query.Select(x => x.Id)
            .ToListAsync();

        query = resolver.Sort(query,
            [new PostSorters(Id: SortDirection.Descending, null, null)]);
        var sortedIds = await query.Select(x => x.Id)
            .ToListAsync();

        Assert.True(defaultIds.SequenceEqual(sortedIds) == false);
        Assert.True(defaultIds.Except(sortedIds).Any() == false);
        Assert.True(defaultIds.Intersect(sortedIds).Count() == 100);

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ORDER BY", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    }
}