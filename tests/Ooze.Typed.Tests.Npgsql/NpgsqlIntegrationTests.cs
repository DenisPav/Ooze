using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Tests.Npgsql;

public class NpgsqlIntegrationTests : IClassFixture<NpgsqlFixture>
{
    private readonly NpgsqlFixture _fixture;

    public NpgsqlIntegrationTests(NpgsqlFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task InsensitiveLike_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        var resolver = _fixture.ServiceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters("%Sample%"));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ILIKE", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }
}