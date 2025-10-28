using Bogus;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ooze.Typed.Filters.Async;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;
using Ooze.Typed.Sorters.Async;

namespace Ooze.Typed.Tests;

public class GenericAsyncOperationResolverTests
{
    [Fact]
    public async Task Should_Return_Same_Query_When_WithQuery_Is_Called()
    {
        var sutInstance = new SUT();

        var resolver = sutInstance.Resolver.WithQuery(sutInstance.Query);
        Assert.NotNull(resolver);

        var resultQuery = await resolver.ApplyAsync();
        
        await sutInstance.FilterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        await sutInstance.SorterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogSorters[]>());
        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());
        
        Assert.True(Equals(sutInstance.Query, resultQuery));
        Assert.IsAssignableFrom<IAsyncOperationResolver<Blog, BlogFilters, BlogSorters>>(resolver);
    }

    [Fact]
    public async Task Should_Not_Call_FilterHandler_When_Filters_Are_Null()
    {
        var sutInstance = new SUT();
        var resultingQuery = await sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Filter(null)
            .ApplyAsync();

        await sutInstance.FilterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        await sutInstance.SorterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogSorters[]>());
        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());

        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public async Task Should_Call_FilterHandler_When_Filters_Are_Present()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(1, string.Empty, null);

        sutInstance.FilterHandler
            .ApplyAsync(sutInstance.Query, filters)
            .Returns(ValueTask.FromResult(sutInstance.Query.Where(b => b.Id == 1)));

        var resultingQuery = await sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Filter(filters)
            .ApplyAsync();

        await sutInstance.FilterHandler
            .Received(1)
            .ApplyAsync(sutInstance.Query, filters);
        await sutInstance.SorterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogSorters[]>());
        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());

        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public async Task Should_Not_Call_SorterHandler_When_Sorters_Are_Null()
    {
        var sutInstance = new SUT();

        var resultingQuery = await sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Sort(null)
            .ApplyAsync();

        await sutInstance.SorterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<IEnumerable<BlogSorters>>());
        await sutInstance.FilterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public async Task Should_Call_SorterHandler_Even_When_Sorters_Are_Empty()
    {
        var sutInstance = new SUT();

        sutInstance.SorterHandler
            .ApplyAsync(sutInstance.Query, Arg.Any<IEnumerable<BlogSorters>>())
            .Returns(ValueTask.FromResult(sutInstance.Query));

        var resultingQuery = await sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Sort([])
            .ApplyAsync();

        await sutInstance.SorterHandler
            .Received(1)
            .ApplyAsync(sutInstance.Query, Arg.Any<IEnumerable<BlogSorters>>());
        await sutInstance.FilterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public async Task Should_Call_SorterHandler_When_Sorters_Are_Present()
    {
        var sutInstance = new SUT();
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];

        sutInstance.SorterHandler
            .ApplyAsync(sutInstance.Query, sorters)
            .Returns(ValueTask.FromResult<IQueryable<Blog>>(sutInstance.Query.OrderBy(b => b.Id)));

        var resultingQuery = await sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Sort(sorters)
            .ApplyAsync();

        await sutInstance.SorterHandler
            .Received(1)
            .ApplyAsync(sutInstance.Query, sorters);
        await sutInstance.FilterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public async Task Should_Not_Call_PagingHandler_When_PagingOptions_Are_Null()
    {
        var sutInstance = new SUT();

        var resultingQuery = await sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Page(null)
            .ApplyAsync();

        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());
        await sutInstance.FilterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        await sutInstance.SorterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogSorters[]>());
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public async Task Should_Call_PagingHandler_When_PagingOptions_Are_Present()
    {
        var sutInstance = new SUT();
        var pagingOptions = new PagingOptions { Page = 1, Size = 10 };

        sutInstance.PagingHandler
            .Apply(sutInstance.Query, pagingOptions)
            .Returns(sutInstance.Query.Skip(10).Take(10));

        var resultingQuery = await sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Page(pagingOptions)
            .ApplyAsync();

        sutInstance.PagingHandler
            .Received(1)
            .Apply(sutInstance.Query, pagingOptions);
        await sutInstance.FilterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        await sutInstance.SorterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogSorters[]>());
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public async Task Should_Chain_Filter_Sort_And_Page_Operations()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Test", null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        var pagingOptions = new PagingOptions { Page = 0, Size = 5 };

        var filteredQuery = sutInstance.Query
            .Where(b => b.Name.Contains("Test"));
        var sortedQuery = filteredQuery.OrderBy(b => b.Id);
        var pagedQuery = sortedQuery.Take(5);

        sutInstance.SorterHandler
            .ApplyAsync(sutInstance.Query, sorters)
            .Returns(ValueTask.FromResult<IQueryable<Blog>>(sortedQuery));
        sutInstance.FilterHandler
            .ApplyAsync(sortedQuery, filters)
            .Returns(ValueTask.FromResult(filteredQuery));
        sutInstance.PagingHandler
            .Apply(filteredQuery, pagingOptions)
            .Returns(pagedQuery);

        var resultingQuery = await sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Sort(sorters)
            .Filter(filters)
            .Page(pagingOptions)
            .ApplyAsync();

        await sutInstance.SorterHandler
            .Received(1)
            .ApplyAsync(sutInstance.Query, sorters);
        await sutInstance.FilterHandler
            .Received(1)
            .ApplyAsync(sortedQuery, filters);
        sutInstance.PagingHandler
            .Received(1)
            .Apply(filteredQuery, pagingOptions);
        Assert.Equal(pagedQuery, resultingQuery);
    }

    [Fact]
    public async Task Should_Return_Original_Query_When_No_Operations_Applied()
    {
        var sutInstance = new SUT();

        var resultingQuery = await sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .ApplyAsync();

        await sutInstance.FilterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        await sutInstance.SorterHandler
            .DidNotReceive()
            .ApplyAsync(Arg.Any<IQueryable<Blog>>(), Arg.Any<IEnumerable<BlogSorters>>());
        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public async Task Should_Throw_When_Query_Not_Set()
    {
        var sutInstance = new SUT();

        await Assert.ThrowsAsync<Exception>(async () => await sutInstance.Resolver
            .Filter(new BlogFilters(1, string.Empty, null))
            .ApplyAsync());
    }

    private class SUT
    {
        public IQueryable<Blog> Query { get; }
        public IAsyncOperationResolver<Blog, BlogFilters, BlogSorters> Resolver { get; }
        public IAsyncFilterHandler<Blog, BlogFilters> FilterHandler { get; }
        public IAsyncSorterHandler<Blog, BlogSorters> SorterHandler { get; }
        public IPagingHandler<Blog> PagingHandler { get; }
        private ILogger<AsyncOperationResolver<Blog, BlogFilters, BlogSorters>> Log { get; }

        public SUT()
        {
            FilterHandler = Substitute.For<IAsyncFilterHandler<Blog, BlogFilters>>();
            SorterHandler = Substitute.For<IAsyncSorterHandler<Blog, BlogSorters>>();
            PagingHandler = Substitute.For<IPagingHandler<Blog>>();
            Log = Substitute.For<ILogger<AsyncOperationResolver<Blog, BlogFilters, BlogSorters>>>();

            var startDate = new DateTime(2024, 1, 1);
            var blogFaker = new Faker<Blog>()
                .RuleFor(b => b.Id, f => f.IndexFaker)
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .RuleFor(b => b.NumberOfComments, f => f.Random.Int(0, 100))
                .RuleFor(b => b.CreatedAt, (f, b) => startDate.AddDays(b.Id));
            var blogs = blogFaker.Generate(50);

            Query = blogs.AsQueryable();
            Resolver = new AsyncOperationResolver<Blog, BlogFilters, BlogSorters>(
                SorterHandler,
                FilterHandler,
                PagingHandler,
                Log);
        }
    }
}