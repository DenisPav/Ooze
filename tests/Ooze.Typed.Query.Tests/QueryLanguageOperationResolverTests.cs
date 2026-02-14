using System.Linq.Expressions;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed.Query.Tests;

public class QueryLanguageOperationResolverTests
{
    [Fact]
    public void Should_Call_RootResolver_Filter_When_Filter_Is_Called()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(1, string.Empty, null);

        sutInstance.RootResolver
            .Filter(sutInstance.Query, filters)
            .Returns(sutInstance.Query.Where(b => b.Id == 1));

        var resultingQuery = sutInstance.Resolver
            .Filter(sutInstance.Query, filters);

        sutInstance.RootResolver
            .Received(1)
            .Filter(sutInstance.Query, filters);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Call_RootResolver_Sort_When_Sort_Is_Called()
    {
        var sutInstance = new SUT();
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];

        sutInstance.RootResolver
            .Sort(sutInstance.Query, sorters)
            .Returns(sutInstance.Query.OrderBy(b => b.Id));

        var resultingQuery = sutInstance.Resolver
            .Sort(sutInstance.Query, sorters);

        sutInstance.RootResolver
            .Received(1)
            .Sort(sutInstance.Query, sorters);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Call_RootResolver_Page_When_Page_Is_Called()
    {
        var sutInstance = new SUT();
        var pagingOptions = new PagingOptions { Page = 1, Size = 10 };

        sutInstance.RootResolver
            .Page(sutInstance.Query, pagingOptions)
            .Returns(sutInstance.Query.Skip(10).Take(10));

        var resultingQuery = sutInstance.Resolver
            .Page(sutInstance.Query, pagingOptions);

        sutInstance.RootResolver
            .Received(1)
            .Page(sutInstance.Query, pagingOptions);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Call_RootResolver_PageWithCursor_When_PageWithCursor_Is_Called()
    {
        var sutInstance = new SUT();
        var pagingOptions = new CursorPagingOptions<int> { After = 5, Size = 10 };

        sutInstance.RootResolver
            .PageWithCursor(sutInstance.Query, Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions)
            .Returns(sutInstance.Query.Where(b => b.Id > 5).Take(10));

        var resultingQuery = sutInstance.Resolver
            .PageWithCursor(sutInstance.Query, b => b.Id, pagingOptions);

        sutInstance.RootResolver
            .Received(1)
            .PageWithCursor(sutInstance.Query, Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Return_Same_Query_When_QueryLanguage_Query_Is_Null()
    {
        var sutInstance = new SUT();
        var resultingQuery = sutInstance.Resolver
            .FilterWithQueryLanguage(sutInstance.Query, null);

        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IQueryLanguageHandler<Blog>));
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Return_Same_Query_When_QueryLanguage_Query_Is_Empty()
    {
        var sutInstance = new SUT();
        var resultingQuery = sutInstance.Resolver
            .FilterWithQueryLanguage(sutInstance.Query, string.Empty);

        sutInstance.ServiceProvider
            .DidNotReceive()
            .GetService(typeof(IQueryLanguageHandler<Blog>));
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Call_QueryHandler_When_QueryLanguage_Query_Is_Present()
    {
        var sutInstance = new SUT();
        const string query = "Name %% \"Test\"";

        sutInstance.QueryHandler
            .Apply(sutInstance.Query, query)
            .Returns(sutInstance.Query.Where(b => b.Name.Contains("Test")));

        var resultingQuery = sutInstance.Resolver
            .FilterWithQueryLanguage(sutInstance.Query, query);

        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IQueryLanguageHandler<Blog>));
        sutInstance.QueryHandler
            .Received(1)
            .Apply(sutInstance.Query, query);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Resolve_QueryHandler_From_ServiceProvider()
    {
        var sutInstance = new SUT();
        const string query = "Id == 1";

        sutInstance.QueryHandler
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<string>())
            .Returns(sutInstance.Query);

        sutInstance.Resolver
            .FilterWithQueryLanguage(sutInstance.Query, query);

        sutInstance.ServiceProvider
            .Received(1)
            .GetService(typeof(IQueryLanguageHandler<Blog>));
    }

    [Fact]
    public void Should_Apply_QueryLanguage_Filter_And_Return_Modified_Query()
    {
        var sutInstance = new SUT();
        const string query = "NumberOfComments >> 50";
        var filteredQuery = sutInstance.Query.Where(b => b.NumberOfComments > 50);

        sutInstance.QueryHandler
            .Apply(sutInstance.Query, query)
            .Returns(filteredQuery);

        var resultingQuery = sutInstance.Resolver
            .FilterWithQueryLanguage(sutInstance.Query, query);

        Assert.Equal(filteredQuery, resultingQuery);
        Assert.NotEqual(sutInstance.Query.Count(), resultingQuery.Count());
    }

    [Fact]
    public void Should_Handle_Multiple_Sequential_Operations_Including_QueryLanguage()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Company", null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        const string query = "NumberOfComments >> 10";
        var pagingOptions = new PagingOptions { Page = 0, Size = 5 };

        var filteredQuery = sutInstance.Query.Where(b => b.Name.Contains("Company"));
        var sortedQuery = filteredQuery.OrderBy(b => b.Id);
        var queryFilteredQuery = sortedQuery.Where(b => b.NumberOfComments > 10);
        var pagedQuery = queryFilteredQuery.Take(5);

        sutInstance.RootResolver
            .Filter(sutInstance.Query, filters)
            .Returns(filteredQuery);
        sutInstance.RootResolver
            .Sort(filteredQuery, sorters)
            .Returns(sortedQuery);
        sutInstance.QueryHandler
            .Apply(sortedQuery, query)
            .Returns(queryFilteredQuery);
        sutInstance.RootResolver
            .Page(queryFilteredQuery, pagingOptions)
            .Returns(pagedQuery);

        var step1 = sutInstance.Resolver
            .Filter(sutInstance.Query, filters);
        var step2 = sutInstance.Resolver
            .Sort(step1, sorters);
        var step3 = sutInstance.Resolver
            .FilterWithQueryLanguage(step2, query);
        var step4 = sutInstance.Resolver
            .Page(step3, pagingOptions);

        Assert.Equal(filteredQuery, step1);
        Assert.Equal(sortedQuery, step2);
        Assert.Equal(queryFilteredQuery, step3);
        Assert.Equal(pagedQuery, step4);
    }

    [Fact]
    public void Should_Delegate_All_Standard_Operations_To_RootResolver()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(1, string.Empty, null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        var pagingOptions = new PagingOptions { Page = 0, Size = 10 };
        var cursorPagingOptions = new CursorPagingOptions<int> { After = 5, Size = 10 };

        sutInstance.RootResolver
            .Filter(Arg.Any<IQueryable<Blog>>(), Arg.Any<BlogFilters>())
            .Returns(sutInstance.Query);
        sutInstance.RootResolver
            .Sort(Arg.Any<IQueryable<Blog>>(), Arg.Any<IEnumerable<BlogSorters>>())
            .Returns(sutInstance.Query);
        sutInstance.RootResolver
            .Page(Arg.Any<IQueryable<Blog>>(), Arg.Any<PagingOptions>())
            .Returns(sutInstance.Query);
        sutInstance.RootResolver
            .PageWithCursor(Arg.Any<IQueryable<Blog>>(), Arg.Any<Expression<Func<Blog, int>>>(), Arg.Any<CursorPagingOptions<int>>())
            .Returns(sutInstance.Query);

        sutInstance.Resolver
            .Filter(sutInstance.Query, filters);
        sutInstance.Resolver
            .Sort(sutInstance.Query, sorters);
        sutInstance.Resolver
            .Page(sutInstance.Query, pagingOptions);
        sutInstance.Resolver
            .PageWithCursor(sutInstance.Query, b => b.Id, cursorPagingOptions);

        sutInstance.RootResolver
            .Received(1)
            .Filter(sutInstance.Query, filters);
        sutInstance.RootResolver
            .Received(1)
            .Sort(sutInstance.Query, sorters);
        sutInstance.RootResolver
            .Received(1)
            .Page(sutInstance.Query, pagingOptions);
        sutInstance.RootResolver
            .Received(1)
            .PageWithCursor(sutInstance.Query, Arg.Any<Expression<Func<Blog, int>>>(), cursorPagingOptions);
    }

    [Fact]
    public void Should_Apply_QueryLanguage_With_Complex_Query()
    {
        var sutInstance = new SUT();
        const string query = "(Name %% \"Company\" || Name %% \"Inc\") && NumberOfComments >> 20";
        var filteredQuery = sutInstance.Query
            .Where(b => (b.Name.Contains("Company") || b.Name.Contains("Inc")) && b.NumberOfComments > 20);

        sutInstance.QueryHandler
            .Apply(sutInstance.Query, query)
            .Returns(filteredQuery);

        var resultingQuery = sutInstance.Resolver
            .FilterWithQueryLanguage(sutInstance.Query, query);

        sutInstance.QueryHandler
            .Received(1)
            .Apply(sutInstance.Query, query);
        Assert.Equal(filteredQuery, resultingQuery);
    }

    private class SUT
    {
        public IQueryable<Blog> Query { get; }
        public IQueryLanguageOperationResolver Resolver { get; }
        public IOperationResolver RootResolver { get; }
        public IServiceProvider ServiceProvider { get; }
        public IQueryLanguageHandler<Blog> QueryHandler { get; }
        private ILogger<QueryLanguageOperationResolver> Log { get; }

        public SUT()
        {
            RootResolver = Substitute.For<IOperationResolver>();
            ServiceProvider = Substitute.For<IServiceProvider>();
            QueryHandler = Substitute.For<IQueryLanguageHandler<Blog>>();
            Log = Substitute.For<ILogger<QueryLanguageOperationResolver>>();

            ServiceProvider
                .GetService(typeof(IQueryLanguageHandler<Blog>))
                .Returns(QueryHandler);

            var startDate = new DateTime(2024, 1, 1);
            var blogFaker = new Faker<Blog>()
                .RuleFor(b => b.Id, f => f.IndexFaker)
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .RuleFor(b => b.NumberOfComments, f => f.Random.Int(0, 100))
                .RuleFor(b => b.CreatedAt, (f, b) => startDate.AddDays(b.Id));
            var blogs = blogFaker.Generate(50);

            Query = blogs.AsQueryable();
            Resolver = new QueryLanguageOperationResolver(RootResolver, ServiceProvider, Log);
        }
    }
}