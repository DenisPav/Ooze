﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;
using Ooze.Typed.Tests.Integration.Setup;

namespace Ooze.Typed.Tests.Integration;

public class DatabaseFilterRangeThanIntegrationTests(DbFixture<DatabaseContext> fixture)
    : IClassFixture<DbFixture<DatabaseContext>>
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
        using var scope = fixture.CreateServiceProvider<PostRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(filter, null, null))
            .Apply();

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
        using var scope = fixture.CreateServiceProvider<PostRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        Assert.Throws<InvalidOperationException>(() => oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(null, filter, null))
            .Apply());
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

        using var scope = fixture.CreateServiceProvider<PostRangeFiltersProvider>().CreateScope();
        var provider = scope.ServiceProvider;

        await using var context = fixture.CreateContext();
        var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostRangeFilters, PostSorters>>();

        IQueryable<Post> query = context.Posts;
        query = oozeResolver.WithQuery(query)
            .Filter(new PostRangeFilters(null, null, filter))
            .Apply();


        var results = await query.ToListAsync();
        Assert.True(results.All(x => x.Date >= filter.From));
        Assert.True(results.All(x => x.Date <= filter.To));
    }
}