using System.Linq.Expressions;
using Bogus;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ooze.Typed.Filters;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests;

public class GenericOperationResolverTests
{
    [Fact]
    public void Should_Return_Same_Query_When_WithQuery_Is_Called()
    {
        var sutInstance = new SUT();

        var resolver = sutInstance.Resolver.WithQuery(sutInstance.Query);
        Assert.NotNull(resolver);
        
        var resultQuery = resolver.Apply();
        Assert.True(Equals(sutInstance.Query, resultQuery));
        Assert.IsAssignableFrom<IOperationResolver<Blog, BlogFilters, BlogSorters>>(resolver);
    }

    [Fact]
    public void Should_Not_Call_FilterHandler_When_Filters_Are_Null()
    {
        var sutInstance = new SUT();
        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Filter(null)
            .Apply();

        sutInstance.FilterHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Call_FilterHandler_When_Filters_Are_Present()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(1, string.Empty, null);

        sutInstance.FilterHandler
            .Apply(sutInstance.Query, filters)
            .Returns(sutInstance.Query.Where(b => b.Id == 1));

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Filter(filters)
            .Apply();

        sutInstance.FilterHandler
            .Received(1)
            .Apply(sutInstance.Query, filters);
        
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Not_Call_SorterHandler_When_Sorters_Are_Null()
    {
        var sutInstance = new SUT();

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Sort(null)
            .Apply();

        sutInstance.SorterHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<IEnumerable<BlogSorters>>());
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Not_Call_SorterHandler_When_Sorters_Are_Empty()
    {
        var sutInstance = new SUT();

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Sort([])
            .Apply();

        sutInstance.SorterHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<IEnumerable<BlogSorters>>());
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Call_SorterHandler_When_Sorters_Are_Present()
    {
        var sutInstance = new SUT();
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];

        sutInstance.SorterHandler
            .Apply(sutInstance.Query, sorters)
            .Returns(sutInstance.Query.OrderBy(b => b.Id));

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Sort(sorters)
            .Apply();

        sutInstance.SorterHandler
            .Received(1)
            .Apply(sutInstance.Query, sorters);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Not_Call_PagingHandler_When_PagingOptions_Are_Null()
    {
        var sutInstance = new SUT();

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Page(null)
            .Apply();

        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Call_PagingHandler_When_PagingOptions_Are_Present()
    {
        var sutInstance = new SUT();
        var pagingOptions = new PagingOptions { Page = 1, Size = 10 };

        sutInstance.PagingHandler
            .Apply(sutInstance.Query, pagingOptions)
            .Returns(sutInstance.Query.Skip(10).Take(10));

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Page(pagingOptions)
            .Apply();

        sutInstance.PagingHandler
            .Received(1)
            .Apply(sutInstance.Query, pagingOptions);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Not_Call_PagingHandler_When_CursorPagingOptions_Are_Null()
    {
        var sutInstance = new SUT();

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .PageWithCursor<int, int>(b => b.Id, null)
            .Apply();

        sutInstance.PagingHandler
            .DidNotReceive()
            .ApplyCursor(
                Arg.Any<IQueryable<Blog>>(),
                Arg.Any<System.Linq.Expressions.Expression<Func<Blog, int>>>(),
                Arg.Any<CursorPagingOptions<int>>());
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Call_PagingHandler_When_CursorPagingOptions_Are_Present()
    {
        var sutInstance = new SUT();
        var pagingOptions = new CursorPagingOptions<int> { After = 5, Size = 10 };

        sutInstance.PagingHandler
            .ApplyCursor(sutInstance.Query, Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions)
            .Returns(sutInstance.Query.Where(b => b.Id > 5).Take(10));

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .PageWithCursor(b => b.Id, pagingOptions)
            .Apply();

        sutInstance.PagingHandler
            .Received(1)
            .ApplyCursor(
                sutInstance.Query,
                Arg.Any<Expression<Func<Blog, int>>>(),
                pagingOptions);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Chain_Filter_Sort_And_Page_Operations()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Test", null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        var pagingOptions = new PagingOptions { Page = 0, Size = 5 };

        var filteredQuery = sutInstance.Query
            .Where(b => b.Name.Contains("Test"));
        var sortedQuery = filteredQuery.OrderBy(b => b.Id);
        var pagedQuery = sortedQuery.Take(5);

        sutInstance.FilterHandler
            .Apply(sutInstance.Query, filters)
            .Returns(filteredQuery);
        sutInstance.SorterHandler
            .Apply(filteredQuery, sorters)
            .Returns(sortedQuery);
        sutInstance.PagingHandler
            .Apply(sortedQuery, pagingOptions)
            .Returns(pagedQuery);

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Filter(filters)
            .Sort(sorters)
            .Page(pagingOptions)
            .Apply();

        sutInstance.FilterHandler
            .Received(1)
            .Apply(sutInstance.Query, filters);
        sutInstance.SorterHandler
            .Received(1)
            .Apply(filteredQuery, sorters);
        sutInstance.PagingHandler
            .Received(1)
            .Apply(sortedQuery, pagingOptions);
        Assert.Equal(pagedQuery, resultingQuery);
    }

    [Fact]
    public void Should_Apply_All_Operations_With_Single_Apply_Call()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Test", null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        var pagingOptions = new PagingOptions { Page = 0, Size = 5 };

        var filteredQuery = sutInstance.Query
            .Where(b => b.Name.Contains("Test"));
        var sortedQuery = filteredQuery.OrderBy(b => b.Id);
        var pagedQuery = sortedQuery.Take(5);

        sutInstance.FilterHandler
            .Apply(sutInstance.Query, filters)
            .Returns(filteredQuery);
        sutInstance.SorterHandler
            .Apply(filteredQuery, sorters)
            .Returns(sortedQuery);
        sutInstance.PagingHandler
            .Apply(sortedQuery, pagingOptions)
            .Returns(pagedQuery);

        var resultingQuery = sutInstance
            .Resolver
            .Apply(sutInstance.Query, sorters, filters, pagingOptions);

        sutInstance.FilterHandler
            .Received(1)
            .Apply(Arg.Any<IQueryable<Blog>>(), filters);
        sutInstance.SorterHandler
            .Received(1)
            .Apply(Arg.Any<IQueryable<Blog>>(), sorters);
        sutInstance.PagingHandler
            .Received(1)
            .Apply(Arg.Any<IQueryable<Blog>>(), pagingOptions);
        Assert.Equal(pagedQuery, resultingQuery);
    }

    [Fact]
    public void Should_Apply_All_Operations_With_Cursor_Paging()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Test", null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        var pagingOptions = new CursorPagingOptions<int> { After = 5, Size = 10 };

        var filteredQuery = sutInstance.Query
            .Where(b => b.Name.Contains("Test"));
        var sortedQuery = filteredQuery.OrderBy(b => b.Id);
        var pagedQuery = sortedQuery.Where(b => b.Id > 5).Take(10);

        sutInstance.FilterHandler
            .Apply(sutInstance.Query, filters)
            .Returns(filteredQuery);
        sutInstance.SorterHandler
            .Apply(filteredQuery, sorters)
            .Returns(sortedQuery);
        sutInstance.PagingHandler
            .ApplyCursor(sortedQuery, Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions)
            .Returns(pagedQuery);

        var resultingQuery = sutInstance.Resolver
            .Apply(sutInstance.Query, sorters, filters, b => b.Id, pagingOptions);

        sutInstance.FilterHandler
            .Received(1)
            .Apply(Arg.Any<IQueryable<Blog>>(), filters);
        sutInstance.SorterHandler
            .Received(1)
            .Apply(Arg.Any<IQueryable<Blog>>(), sorters);
        sutInstance.PagingHandler
            .Received(1)
            .ApplyCursor(
                Arg.Any<IQueryable<Blog>>(),
                Arg.Any<Expression<Func<Blog, int>>>(),
                pagingOptions);
        Assert.Equal(pagedQuery, resultingQuery);
    }

    [Fact]
    public void Should_Return_Original_Query_When_No_Operations_Applied()
    {
        var sutInstance = new SUT();

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Query)
            .Apply();

        sutInstance.FilterHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>());
        sutInstance.SorterHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<IEnumerable<BlogSorters>>());
        sutInstance.PagingHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>());
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    private class SUT
    {
        public IQueryable<Blog> Query { get; }
        public IOperationResolver<Blog, BlogFilters, BlogSorters> Resolver { get; }
        public IFilterHandler<Blog, BlogFilters> FilterHandler { get; }
        public ISorterHandler<Blog, BlogSorters> SorterHandler { get; }
        public IPagingHandler<Blog> PagingHandler { get; }
        private ILogger<OperationResolver<Blog, BlogFilters, BlogSorters>> Log { get; }

        public SUT()
        {
            FilterHandler = Substitute.For<IFilterHandler<Blog, BlogFilters>>();
            SorterHandler = Substitute.For<ISorterHandler<Blog, BlogSorters>>();
            PagingHandler = Substitute.For<IPagingHandler<Blog>>();
            Log = Substitute.For<ILogger<OperationResolver<Blog, BlogFilters, BlogSorters>>>();

            var startDate = new DateTime(2024, 1, 1);
            var blogFaker = new Faker<Blog>()
                .RuleFor(b => b.Id, f => f.IndexFaker)
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .RuleFor(b => b.NumberOfComments, f => f.Random.Int(0, 100))
                .RuleFor(b => b.CreatedAt, (f, b) => startDate.AddDays(b.Id));
            var blogs = blogFaker.Generate(50);

            Query = blogs.AsQueryable();
            Resolver = new OperationResolver<Blog, BlogFilters, BlogSorters>(
                SorterHandler,
                FilterHandler,
                PagingHandler,
                Log);
        }
    }
}