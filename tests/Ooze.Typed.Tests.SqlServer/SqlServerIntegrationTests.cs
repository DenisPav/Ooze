using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Tests.SqlServer;

public class SqlServerIntegrationTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public SqlServerIntegrationTests(SqlServerFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task IsDate_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(true, null, null, null, null, null, null, null, null, null, null, null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ISDATE", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task IsNumeric_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(null, true, null, null, null, null, null, null, null, null, null, null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ISNUMERIC", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffDay_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null, null, null, null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(day,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffMonth_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null, null, null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(month,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffYear_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null, null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(year,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffWeek_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(week,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffHour_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(hour,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffMinute_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(minute,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffSecond_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(second,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffMillisecond_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, null, null, new DateTime(2022, 2, 2, 20, 20, 22), new DateTime(2022, 2, 2), null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(millisecond,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffMicrosecond_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, null, null, null, new DateTime(2022, 2, 2, 20, 20, 22), new DateTime(2022, 2, 2, 20, 20, 22), null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(microsecond,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffNanosecond_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = _fixture.CreateContext();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, null, null, null, new DateTime(2022, 2, 2, 20, 20, 22), null, new DateTime(2022, 2, 2, 20, 20, 22)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(nanosecond,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }
}