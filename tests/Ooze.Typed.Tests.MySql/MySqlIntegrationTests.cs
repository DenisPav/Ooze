using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Tests.MySql;

public class MySqlIntegrationTests : IClassFixture<MySqlFixture>
{
    private readonly MySqlFixture _fixture;

    public MySqlIntegrationTests(MySqlFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task DateDiffDay_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
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
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
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
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
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
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
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
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
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
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
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
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffMicrosecondFilter: new DateTime(2022, 2, 2, 20, 20, 22)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(MICROSECOND,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }
}