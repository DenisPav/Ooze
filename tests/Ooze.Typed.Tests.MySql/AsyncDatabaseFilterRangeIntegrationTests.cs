using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;
using Ooze.Typed.Tests.MySql.Setup;
using Ooze.Typed.Tests.MySql.Setup.Async;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterRangeThanIntegrationTests(MySqlFixture fixture)
    : IClassFixture<MySqlFixture>
{
    [Theory]
    [InlineData(-10, 5)]
    [InlineData(5, 20)]
    [InlineData(25, 90)]
    [InlineData(95, 120)]
    public async Task Should_Correctly_Filter_Data_By_Range_Int_Filter(
        int from,
        int to)
    {
        var filter = new RangeFilter<long> { From = from, To = to };
        using var scope = fixture.CreateServiceProvider<AsyncPostRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(filter, null, null))
            .ApplyAsync();

        var results = await query.ToListAsync();
        Assert.True(results.All(x => x.Id >= filter.From));
        Assert.True(results.All(x => x.Id <= filter.To));
    }

    [Theory]
    [InlineData("-10", "5")]
    [InlineData("postname", "endpostname")]
    public async Task Should_Correctly_Filter_Data_By_Range_String_Filter(string from, string to)
    {
        var filter = new RangeFilter<string> { From = from, To = to };
        using var scope = fixture.CreateServiceProvider<AsyncPostRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(null, filter, null))
            .ApplyAsync());
    }

    [Fact]
    public async Task Should_Correctly_Filter_Data_By_Range_DateTime_Filter()
    {
        var from = new DateTime(2022, 1, 1);
        var to = new DateTime(2022, 2, 20);
        var filter = new RangeFilter<DateTime>
        {
            From = from,
            To = to
        };

        using var scope = fixture.CreateServiceProvider<AsyncPostRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(null, null, filter))
            .ApplyAsync();


        var results = await query.ToListAsync();
        Assert.True(results.All(x => x.Date >= filter.From));
        Assert.True(results.All(x => x.Date <= filter.To));
    }
}