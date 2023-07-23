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
            new PostFilters(DateFilter: new DateTime(2022, 5, 20)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("TIMESTAMPDIFF(DAY,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }
}