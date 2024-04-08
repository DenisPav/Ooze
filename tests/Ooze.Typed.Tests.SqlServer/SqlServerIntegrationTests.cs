using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;
using Ooze.Typed.Sorters;
using Ooze.Typed.Tests.SqlServer.OozeConfiguration;

namespace Ooze.Typed.Tests.SqlServer;

public class SqlServerIntegrationTests(SqlServerFixture fixture) : IClassFixture<SqlServerFixture>
{
    [Theory]
    [InlineData(10)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task Equal_Should_Update_Query_And_Return_Correct_Query(int postId)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(PostId: postId));

        var filteredItemsCount = await query.CountAsync();
        Assert.True(filteredItemsCount == 1);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task NotEqual_Should_Update_Query_And_Return_Correct_Query(int postId)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(NotEqualPostId: postId));

        var containsPostId = await query.AnyAsync(post => post.Id == postId);
        Assert.True(containsPostId == false);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task GreaterThan_Should_Update_Query_And_Return_Correct_Query(int postId)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(GreaterThanPostId: postId));

        var filteredItemsCount = await query.CountAsync();
        var expectedCount = SqlServerContext.TotalRecords - postId;
        Assert.True(filteredItemsCount == expectedCount);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task LessThan_Should_Update_Query_And_Return_Correct_Query(int postId)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(LessThanPostId: postId));

        var filteredItemsCount = await query.CountAsync();
        var expectedCount = postId - 1;
        Assert.True(filteredItemsCount == expectedCount);
    }

    [Fact]
    public async Task In_Should_Update_Query_And_Return_Correct_Query()
    {
        var postIds = new long[] { 1, 10, 100 };
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(IdIn: postIds));

        var materializedIds = await query.Select(x => x.Id)
            .ToListAsync();
        var containsAll = materializedIds.SequenceEqual(postIds);

        Assert.True(containsAll == true);
    }

    [Fact]
    public async Task NotIn_Should_Update_Query_And_Return_Correct_Query()
    {
        var postIds = new long[] { -1, 1000, -100 };
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(IdIn: postIds));

        var materializedIds = await query.Select(x => x.Id)
            .ToListAsync();
        var containsAny = materializedIds.Intersect(postIds).Any();

        Assert.True(containsAny == false);
    }

    [Theory]
    [InlineData(-200, 1000)]
    [InlineData(1, 100)]
    [InlineData(50, 50)]
    [InlineData(30, 101)]
    public async Task Range_Should_Update_Query_And_Return_Correct_Query(
        long from,
        long to)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(IdRange: new RangeFilter<long>
        {
            From = from,
            To = to
        }));

        var materializedIds = await query.Select(x => x.Id)
            .ToListAsync();
        var allIdsValid = materializedIds.All(x => x >= from && x <= to);

        Assert.True(allIdsValid == true);
    }

    [Theory]
    [InlineData(-200, 1000)]
    [InlineData(1, 100)]
    [InlineData(50, 50)]
    [InlineData(30, 101)]
    public async Task OutOfRange_Should_Update_Query_And_Return_Correct_Query(
        long from,
        long to)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(IdOutOfRange: new RangeFilter<long>
        {
            From = from,
            To = to
        }));

        var materializedIds = await query.Select(x => x.Id)
            .ToListAsync();
        var allIdsValid = materializedIds.All(x => x < from || x > to);

        Assert.True(allIdsValid == true);
    }

    [Theory]
    [InlineData("1_Sample")]
    [InlineData("12_Sample")]
    [InlineData("50_Sample")]
    public async Task StartsWith_Should_Update_Query_And_Return_Correct_Query(string prefix)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(NameStartsWith: prefix));

        var materialized = await query.ToListAsync();
        var allEntitiesValid = materialized.All(x => x.Name.StartsWith(prefix));

        Assert.True(allEntitiesValid == true);
    }

    [Theory]
    [InlineData("dlkjsad")]
    [InlineData("3213")]
    [InlineData("$!#")]
    public async Task DoesntStartWith_Should_Update_Query_And_Return_Correct_Query(string prefix)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(NameDoesntWith: prefix));

        var materialized = await query.ToListAsync();
        var allEntitiesValid = materialized.All(x => x.Name.StartsWith(prefix) == false);

        Assert.True(allEntitiesValid == true);
    }

    [Theory]
    [InlineData("post_1")]
    [InlineData("post_12")]
    [InlineData("post_50")]
    public async Task EndsWith_Should_Update_Query_And_Return_Correct_Query(string suffix)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(NameEndsWith: suffix));

        var materialized = await query.ToListAsync();
        var allEntitiesValid = materialized.All(x => x.Name.EndsWith(suffix));

        Assert.True(allEntitiesValid == true);
    }

    [Theory]
    [InlineData("dlkjsad")]
    [InlineData("3213")]
    [InlineData("$!#")]
    public async Task DoesntEndWith_Should_Update_Query_And_Return_Correct_Query(string suffix)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(NameDoesntEndWith: suffix));

        var materialized = await query.ToListAsync();
        var allEntitiesValid = materialized.All(x => x.Name.EndsWith(suffix) == false);

        Assert.True(allEntitiesValid == true);
    }

    [Fact]
    public async Task IsDate_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(IsNameDate: true));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ISDATE", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task IsNumeric_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(IsIdNumeric: true));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ISNUMERIC", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffDay_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffDay: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(day,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffMonth_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffMonth: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(month,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffYear_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffYear: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(year,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffWeek_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffWeek: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(week,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffHour_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffHour: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(hour,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffMinute_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffMinute: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(minute,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffSecond_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffSecond: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(second,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffMillisecond_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffMillisecond: new DateTime(2022, 2, 2, 20, 20, 22),
                DateDiffDayEqual: new DateTime(2022, 2, 2)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(millisecond,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffMicrosecond_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffMicrosecond: new DateTime(2022, 2, 2, 20, 20, 22),
                DateDiffDayEqual: new DateTime(2022, 2, 2, 20, 20, 22)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(microsecond,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    [Fact]
    public async Task DateDiffNanosecond_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostFiltersProvider>().GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(DateDiffDayEqual: new DateTime(2022, 2, 2, 20, 20, 22),
                DateDiffNanosecond: new DateTime(2022, 2, 2, 20, 20, 22)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(nanosecond,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
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