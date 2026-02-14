using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Tests.Base;
using Ooze.Typed.Tests.Base.Setup;
using Ooze.Typed.Tests.Npgsql.Setup;

namespace Ooze.Typed.Tests.Npgsql;

public class NpgsqlAsyncIntegrationTests(NpgsqlFixture fixture)
{
    #region NonAsync

    [Fact]
    public async Task InsensitiveLike_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = GenericDbFixture.CreateServiceProvider<PsqlPostFiltersProvider>()
            .GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostPsqlFilters("%Sample%", null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ILIKE", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task SoundexEqual_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = GenericDbFixture.CreateServiceProvider<PsqlPostFiltersProvider>()
            .GetRequiredService<IOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostPsqlFilters(null, "%Sample%"));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("soundex", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    #endregion
    
    #region Async
    
    [Fact]
    public async Task InsensitiveLike_Should_Update_Query_And_Return_Correct_Query_Async()
    {
        await using var context = fixture.CreateContext();

        var resolver = GenericDbFixture.CreateServiceProvider<PsqlPostFiltersProvider>()
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = await resolver.FilterAsync(query, new PostPsqlFilters("%Sample%", null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ILIKE", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems);
    }

    [Fact]
    public async Task SoundexEqual_Should_Update_Query_And_Return_Correct_Query_Async()
    {
        await using var context = fixture.CreateContext();

        var resolver = GenericDbFixture.CreateServiceProvider<PsqlPostFiltersProvider>()
            .GetRequiredService<IAsyncOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = await resolver.FilterAsync(query, new PostPsqlFilters(null, "%Sample%"));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("soundex", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.False(hasFilteredItems);
    }
    
    #endregion
}