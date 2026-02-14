using System.Linq.Expressions;
using Bogus;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ooze.Typed.Filters.Async;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;
using Ooze.Typed.Sorters.Async;

namespace Ooze.Typed.Tests;

public class AsyncOperationResolverTests
{
    [Fact]
    public async Task Should_Return_Same_Query_When_Filters_Are_Null()
    {
        var sutInstance = new SUT();
        var resultingQuery = await sutInstance.Resolver
            .FilterAsync<Blog, BlogFilters?>(sutInstance.Query, null);

        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
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
            .FilterAsync(sutInstance.Query, filters);

        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IPagingHandler<Blog>));
        await sutInstance.FilterHandler
            .Received(1)
            .ApplyAsync(sutInstance.Query, filters);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public async Task Should_Return_Same_Query_When_Sorters_Are_Null()
    {
        var sutInstance = new SUT();
        var resultingQuery = await sutInstance.Resolver
            .SortAsync<Blog, BlogSorters>(sutInstance.Query, null);

        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IPagingHandler<Blog>));
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public async Task Should_Return_Same_Query_When_Sorters_Are_Empty()
    {
        var sutInstance = new SUT();
        var resultingQuery = await sutInstance.Resolver
            .SortAsync<Blog, BlogSorters>(sutInstance.Query, []);

        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IPagingHandler<Blog>));
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
            .SortAsync(sutInstance.Query, sorters);

        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IPagingHandler<Blog>));
        
        await sutInstance.SorterHandler
            .Received(1)
            .ApplyAsync(sutInstance.Query, sorters);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public async Task Should_Return_Same_Query_When_PagingOptions_Are_Null()
    {
        var sutInstance = new SUT();
        var resultingQuery = await sutInstance.Resolver
            .PageAsync(sutInstance.Query, null);

        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IPagingHandler<Blog>));
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
            .PageAsync(sutInstance.Query, pagingOptions);

        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IPagingHandler<Blog>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.PagingHandler
            .Received(1)
            .Apply(sutInstance.Query, pagingOptions);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public async Task Should_Return_Same_Query_When_CursorPagingOptions_Are_Null()
    {
        var sutInstance = new SUT();
        var resultingQuery = await sutInstance.Resolver
            .PageWithCursorAsync<Blog, int, int>(
                sutInstance.Query,
                b => b.Id,
                null);

        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IPagingHandler<Blog>));
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public async Task Should_Call_PagingHandler_When_CursorPagingOptions_Are_Present()
    {
        var sutInstance = new SUT();
        var pagingOptions = new CursorPagingOptions<int> { After = 5, Size = 10 };

        sutInstance.PagingHandler
            .ApplyCursor(sutInstance.Query, Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions)
            .Returns(sutInstance.Query.Where(b => b.Id > 5).Take(10));

        var resultingQuery = await sutInstance.Resolver
            .PageWithCursorAsync(
                sutInstance.Query,
                b => b.Id,
                pagingOptions);

        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IPagingHandler<Blog>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.PagingHandler
            .Received(1)
            .ApplyCursor(
                sutInstance.Query,
                Arg.Any<Expression<Func<Blog, int>>>(),
                pagingOptions);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public async Task Should_Apply_Filter_And_Return_Modified_Query()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Company", null);
        var filteredQuery = sutInstance.Query
            .Where(b => b.Name.Contains("Company"));

        sutInstance.FilterHandler
            .ApplyAsync(sutInstance.Query, filters)
            .Returns(ValueTask.FromResult(filteredQuery));
        
        var resultingQuery = await sutInstance.Resolver
            .FilterAsync(sutInstance.Query, filters);
        
        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IPagingHandler<Blog>));

        Assert.Equal(filteredQuery, resultingQuery);
        Assert.NotEqual(sutInstance.Query.Count(), resultingQuery.Count());
    }

    [Fact]
    public async Task Should_Apply_Sort_And_Return_Modified_Query()
    {
        var sutInstance = new SUT();
        BlogSorters[] sorters = [new(null, SortDirection.Descending, null)];
        var sortedQuery = sutInstance.Query
            .OrderByDescending(b => b.Name);

        sutInstance.SorterHandler
            .ApplyAsync(sutInstance.Query, sorters)
            .Returns(ValueTask.FromResult<IQueryable<Blog>>(sortedQuery));

        var resultingQuery = await sutInstance.Resolver
            .SortAsync(sutInstance.Query, sorters);
        
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IPagingHandler<Blog>));

        Assert.Equal(sortedQuery, resultingQuery);
        Assert.NotEqual(sutInstance.Query.First().Name, resultingQuery.First().Name);
    }

    [Fact]
    public async Task Should_Apply_Paging_And_Return_Modified_Query()
    {
        var sutInstance = new SUT();
        var pagingOptions = new PagingOptions { Page = 1, Size = 10 };
        var pagedQuery = sutInstance.Query
            .Skip(10)
            .Take(10);

        sutInstance.PagingHandler
            .Apply(sutInstance.Query, pagingOptions)
            .Returns(pagedQuery);

        var resultingQuery = await sutInstance.Resolver
            .PageAsync(sutInstance.Query, pagingOptions);
        
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IPagingHandler<Blog>));

        Assert.Equal(pagedQuery, resultingQuery);
        Assert.Equal(10, resultingQuery.Count());
    }

    [Fact]
    public async Task Should_Apply_Cursor_Paging_And_Return_Modified_Query()
    {
        var sutInstance = new SUT();
        var pagingOptions = new CursorPagingOptions<int> { After = 10, Size = 15 };
        var pagedQuery = sutInstance.Query
            .Where(b => b.Id > 10)
            .Take(15);

        sutInstance.PagingHandler
            .ApplyCursor(sutInstance.Query, Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions)
            .Returns(pagedQuery);

        var resultingQuery = await sutInstance.Resolver
            .PageWithCursorAsync(sutInstance.Query, b => b.Id, pagingOptions);
        
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IPagingHandler<Blog>));

        Assert.Equal(pagedQuery, resultingQuery);
        Assert.Equal(15, resultingQuery.Count());
        Assert.All(resultingQuery, b => Assert.True(b.Id > 10));
    }

    [Fact]
    public async Task Should_Handle_Multiple_Sequential_Operations()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Company", null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        var pagingOptions = new PagingOptions { Page = 0, Size = 5 };

        var filteredQuery = sutInstance.Query
            .Where(b => b.Name.Contains("Company"));
        var sortedQuery = filteredQuery.OrderBy(b => b.Id);
        var pagedQuery = sortedQuery.Take(5);

        sutInstance.FilterHandler
            .ApplyAsync(sutInstance.Query, filters)
            .Returns(ValueTask.FromResult(filteredQuery));
        sutInstance.SorterHandler
            .ApplyAsync(filteredQuery, sorters)
            .Returns(ValueTask.FromResult<IQueryable<Blog>>(sortedQuery));
        sutInstance.PagingHandler
            .Apply(sortedQuery, pagingOptions)
            .Returns(pagedQuery);

        var step1 = await sutInstance.Resolver
            .FilterAsync(sutInstance.Query, filters);
        var step2 = await sutInstance.Resolver
            .SortAsync(step1, sorters);
        var step3 = await sutInstance.Resolver
            .PageAsync(step2, pagingOptions);
        
        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>));
        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>));
        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IPagingHandler<Blog>));

        Assert.Equal(filteredQuery, step1);
        Assert.Equal(sortedQuery, step2);
        Assert.Equal(pagedQuery, step3);
    }

    private class SUT
    {
        public IQueryable<Blog> Query { get; }
        public IAsyncOperationResolver Resolver { get; }
        public IServiceProvider ServiceProvider { get; }
        public IAsyncFilterHandler<Blog, BlogFilters> FilterHandler { get; }
        public IAsyncSorterHandler<Blog, BlogSorters> SorterHandler { get; }
        public IPagingHandler<Blog> PagingHandler { get; }
        private ILogger<AsyncOperationResolver> Log { get; }

        public SUT()
        {
            ServiceProvider = Substitute.For<IServiceProvider>();
            FilterHandler = Substitute.For<IAsyncFilterHandler<Blog, BlogFilters>>();
            SorterHandler = Substitute.For<IAsyncSorterHandler<Blog, BlogSorters>>();
            PagingHandler = Substitute.For<IPagingHandler<Blog>>();
            Log = Substitute.For<ILogger<AsyncOperationResolver>>();

            ServiceProvider
                .GetService(typeof(IAsyncFilterHandler<Blog, BlogFilters>))
                .Returns(FilterHandler);
            ServiceProvider
                .GetService(typeof(IAsyncSorterHandler<Blog, BlogSorters>))
                .Returns(SorterHandler);
            ServiceProvider
                .GetService(typeof(IPagingHandler<Blog>))
                .Returns(PagingHandler);

            var startDate = new DateTime(2024, 1, 1);
            var blogFaker = new Faker<Blog>()
                .RuleFor(b => b.Id, f => f.IndexFaker)
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .RuleFor(b => b.NumberOfComments, f => f.Random.Int(0, 100))
                .RuleFor(b => b.CreatedAt, (f, b) => startDate.AddDays(b.Id));
            var blogs = blogFaker.Generate(50);

            Query = blogs.AsQueryable();
            Resolver = new AsyncOperationResolver(ServiceProvider, Log);
        }
    }
}