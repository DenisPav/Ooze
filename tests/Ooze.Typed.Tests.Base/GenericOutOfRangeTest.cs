using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;
using Ooze.Typed.Tests.Base.Setup;
using Ooze.Typed.Tests.Base.Setup.Async;

namespace Ooze.Typed.Tests.Base;

public abstract class GenericOutOfRangeTest<TFixture>(TFixture fixture) : GenericTest<TFixture>
    where TFixture : DbFixture
{
    #region NonAsync

    [Theory]
    [InlineData(-10, 5)]
    [InlineData(5, 20)]
    [InlineData(25, 90)]
    [InlineData(95, 120)]
    public async Task Should_Correctly_Filter_Data_By_Out_Of_Range_Int_Filter(
        int from,
        int to)
    {
        var filter = new RangeFilter<long> { From = from, To = to };
        using var scope = DbFixture.CreateServiceProvider<PostOutOfRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(filter, null, null))
            .Apply();

        var results = await query.ToListAsync();
        var generatedRange = Enumerable.Range(from, to + 1).Select(x => (long)x);
        Assert.Contains(results, x => generatedRange.Contains(x.Id) == false);
    }

    [Theory]
    [InlineData("-10", "5")]
    [InlineData("postname", "endpostname")]
    public async Task Should_Correctly_Filter_Data_By_Out_Of_Range_String_Filter(string from, string to)
    {
        var filter = new RangeFilter<string> { From = from, To = to };
        using var scope = DbFixture.CreateServiceProvider<PostOutOfRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        Assert.Throws<InvalidOperationException>(() => oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(null, filter, null))
            .Apply());
    }

    [Fact]
    public async Task Should_Correctly_Filter_Data_By_Out_Of_Range_DateTime_Filter()
    {
        var from = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2022, 2, 20, 0, 0, 0, DateTimeKind.Utc);
        var filter = new RangeFilter<DateTime>
        {
            From = from,
            To = to
        };

        using var scope = DbFixture.CreateServiceProvider<PostOutOfRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOperationResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(null, null, filter))
            .Apply();

        var results = await query.ToListAsync();
        var diffDays = to.Subtract(from).Days;
        var generatedRange = Enumerable.Range(0, diffDays + 1).Select(x => from.AddDays(x));
        Assert.Contains(results, x => generatedRange.Contains(x.Date) == false);
    }

    #endregion
    
    #region Async
    
    [Theory]
    [InlineData(-10, 5)]
    [InlineData(5, 20)]
    [InlineData(25, 90)]
    [InlineData(95, 120)]
    public async Task Should_Correctly_Filter_Data_By_Out_Of_Range_Int_Filter_Async(
        int from,
        int to)
    {
        var filter = new RangeFilter<long> { From = from, To = to };
        using var scope = DbFixture.CreateServiceProvider<AsyncPostOutOfRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(filter, null, null))
            .ApplyAsync();

        var results = await query.ToListAsync();
        var generatedRange = Enumerable.Range(from, to + 1).Select(x => (long)x);
        Assert.Contains(results, x => generatedRange.Contains(x.Id) == false);
    }

    [Theory]
    [InlineData("-10", "5")]
    [InlineData("postname", "endpostname")]
    public async Task Should_Correctly_Filter_Data_By_Out_Of_Range_String_Filter_Async(
        string from, 
        string to)
    {
        var filter = new RangeFilter<string> { From = from, To = to };
        using var scope = DbFixture.CreateServiceProvider<AsyncPostOutOfRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(null, filter, null))
            .ApplyAsync());
    }

    [Fact]
    public async Task Should_Correctly_Filter_Data_By_Out_Of_Range_DateTime_Filter_Async()
    {
        var from = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2022, 2, 20, 0, 0, 0, DateTimeKind.Utc);
        var filter = new RangeFilter<DateTime>
        {
            From = from,
            To = to
        };

        using var scope = DbFixture.CreateServiceProvider<AsyncPostOutOfRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Set<Post>();
        query = await oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(null, null, filter))
            .ApplyAsync();

        var results = await query.ToListAsync();
        var diffDays = to.Subtract(from).Days;
        var generatedRange = Enumerable.Range(0, diffDays + 1).Select(x => from.AddDays(x));
        Assert.Contains(results, x => generatedRange.Contains(x.Date) == false);
    }
    
    #endregion
}