using System.Linq.Expressions;
using Bogus;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed.Query.Tests;

public class GenericQueryLanguageOperationResolverTests
{
    [Fact]
    public void Should_Call_RootResolver_Filter_When_Filter_Is_Called()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(1, string.Empty, null);

        sutInstance.RootResolver
            .Filter(filters)
            .Returns(sutInstance.RootResolver);

        var resolver = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .Filter(filters);

        sutInstance.RootResolver
            .Received(1)
            .Filter(filters);
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Should_Call_RootResolver_Sort_When_Sort_Is_Called()
    {
        var sutInstance = new SUT();
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];

        sutInstance.RootResolver
            .Sort(sorters)
            .Returns(sutInstance.RootResolver);

        var resolver = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .Sort(sorters);

        sutInstance.RootResolver
            .Received(1)
            .Sort(sorters);
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Should_Call_RootResolver_Page_When_Page_Is_Called()
    {
        var sutInstance = new SUT();
        var pagingOptions = new PagingOptions { Page = 1, Size = 10 };

        sutInstance.RootResolver
            .Page(pagingOptions)
            .Returns(sutInstance.RootResolver);

        var resolver = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .Page(pagingOptions);

        sutInstance.RootResolver
            .Received(1)
            .Page(pagingOptions);
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Should_Call_RootResolver_PageWithCursor_When_PageWithCursor_Is_Called()
    {
        var sutInstance = new SUT();
        var pagingOptions = new CursorPagingOptions<int> { After = 5, Size = 10 };

        sutInstance.RootResolver
            .PageWithCursor(Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions)
            .Returns(sutInstance.RootResolver);

        var resolver = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .PageWithCursor(b => b.Id, pagingOptions);

        sutInstance.RootResolver
            .Received(1)
            .PageWithCursor(Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions);
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Should_Not_Call_QueryHandler_When_Query_Is_Null()
    {
        var sutInstance = new SUT();
        sutInstance.RootResolver
            .Apply()
            .Returns(sutInstance.Queryable);

        var resolver = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .FilterWithQueryLanguage(null);

        sutInstance.QueryHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<string>());
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Should_Not_Call_QueryHandler_When_Query_Is_Empty()
    {
        var sutInstance = new SUT();
        sutInstance.RootResolver
            .Apply()
            .Returns(sutInstance.Queryable);

        var resolver = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .FilterWithQueryLanguage(string.Empty);

        sutInstance.QueryHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<string>());
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Should_Call_QueryHandler_When_Query_Is_Present()
    {
        var sutInstance = new SUT();
        const string query = "Name %% \"Test\"";

        sutInstance.RootResolver
            .Apply()
            .Returns(sutInstance.Queryable);
        sutInstance.RootResolver
            .WithQuery(Arg.Any<IQueryable<Blog>>())
            .Returns(sutInstance.RootResolver);

        sutInstance.QueryHandler
            .Apply(sutInstance.Queryable, query)
            .Returns(sutInstance.Queryable.Where(b => b.Name.Contains("Test")));

        var resolver = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .FilterWithQueryLanguage(query);

        sutInstance.QueryHandler
            .Received(1)
            .Apply(sutInstance.Queryable, query);
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Should_Chain_All_Operations_Including_QueryLanguage()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Company", null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        const string query = "NumberOfComments >> 10";
        var pagingOptions = new PagingOptions { Page = 0, Size = 5 };

        var filteredQuery = sutInstance.Queryable
            .Where(b => b.Name.Contains("Company"));
        var sortedQuery = filteredQuery.OrderBy(b => b.Id);
        var queryFilteredQuery = sortedQuery.Where(b => b.NumberOfComments > 10);
        var pagedQuery = queryFilteredQuery.Take(5);

        sutInstance.RootResolver
            .WithQuery(sutInstance.Queryable)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Filter(filters)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Sort(sorters)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Page(pagingOptions)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Apply()
            .Returns(sortedQuery, pagedQuery);
        sutInstance.RootResolver
            .WithQuery(queryFilteredQuery)
            .Returns(sutInstance.RootResolver);

        sutInstance.QueryHandler
            .Apply(sortedQuery, query)
            .Returns(queryFilteredQuery);

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .Filter(filters)
            .Sort(sorters)
            .FilterWithQueryLanguage(query)
            .Page(pagingOptions)
            .Apply();

        sutInstance.RootResolver
            .Received(1)
            .Filter(filters);
        sutInstance.RootResolver
            .Received(1)
            .Sort(sorters);
        sutInstance.QueryHandler
            .Received(1)
            .Apply(sortedQuery, query);
        sutInstance.RootResolver
            .Received(1)
            .Page(pagingOptions);
        Assert.Equal(pagedQuery, resultingQuery);
    }

    [Fact]
    public void Should_Apply_All_Operations_With_Single_Apply_Call()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Test", null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        const string query = "NumberOfComments >> 5";
        var pagingOptions = new PagingOptions { Page = 0, Size = 5 };

        var filteredQuery = sutInstance.Queryable
            .Where(b => b.Name.Contains("Test"));
        var sortedQuery = filteredQuery.OrderBy(b => b.Id);
        var queryFilteredQuery = sortedQuery.Where(b => b.NumberOfComments > 5);
        var pagedQuery = queryFilteredQuery.Take(5);

        sutInstance.RootResolver
            .WithQuery(sutInstance.Queryable)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Sort(sorters)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Filter(filters)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Page(pagingOptions)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Apply()
            .Returns(sortedQuery, pagedQuery);
        sutInstance.RootResolver
            .WithQuery(queryFilteredQuery)
            .Returns(sutInstance.RootResolver);

        sutInstance.QueryHandler
            .Apply(sortedQuery, query)
            .Returns(queryFilteredQuery);

        var resultingQuery = sutInstance.Resolver
            .Apply(sutInstance.Queryable, sorters, filters, query, pagingOptions);

        sutInstance.RootResolver
            .Received(1)
            .Sort(sorters);
        sutInstance.RootResolver
            .Received(1)
            .Filter(filters);
        sutInstance.QueryHandler
            .Received(1)
            .Apply(sortedQuery, query);
        sutInstance.RootResolver
            .Received(1)
            .Page(pagingOptions);
        Assert.Equal(pagedQuery, resultingQuery);
    }

    [Fact]
    public void Should_Apply_All_Operations_With_Cursor_Paging()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(null, "Test", null);
        BlogSorters[] sorters = [new(SortDirection.Ascending, null, null)];
        const string query = "NumberOfComments >> 5";
        var pagingOptions = new CursorPagingOptions<int> { After = 10, Size = 10 };

        var filteredQuery = sutInstance.Queryable
            .Where(b => b.Name.Contains("Test"));
        var sortedQuery = filteredQuery.OrderBy(b => b.Id);
        var queryFilteredQuery = sortedQuery.Where(b => b.NumberOfComments > 5);
        var pagedQuery = queryFilteredQuery.Where(b => b.Id > 10).Take(10);

        sutInstance.RootResolver
            .WithQuery(sutInstance.Queryable)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Sort(sorters)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Filter(filters)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .PageWithCursor(Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Apply()
            .Returns(sortedQuery, pagedQuery);
        sutInstance.RootResolver
            .WithQuery(queryFilteredQuery)
            .Returns(sutInstance.RootResolver);

        sutInstance.QueryHandler
            .Apply(sortedQuery, query)
            .Returns(queryFilteredQuery);

        var resultingQuery = sutInstance.Resolver
            .Apply(sutInstance.Queryable, sorters, filters, query, b => b.Id, pagingOptions);

        sutInstance.RootResolver
            .Received(1)
            .Sort(sorters);
        sutInstance.RootResolver
            .Received(1)
            .Filter(filters);
        sutInstance.QueryHandler
            .Received(1)
            .Apply(sortedQuery, query);
        sutInstance.RootResolver
            .Received(1)
            .PageWithCursor(Arg.Any<Expression<Func<Blog, int>>>(), pagingOptions);
        Assert.Equal(pagedQuery, resultingQuery);
    }

    [Fact]
    public void Should_Return_Original_Query_When_No_Operations_Applied()
    {
        var sutInstance = new SUT();
        sutInstance.RootResolver
            .Apply()
            .Returns(sutInstance.Queryable);

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .Apply();

        sutInstance.RootResolver
            .DidNotReceive()
            .Filter(Arg.Any<BlogFilters>());
        sutInstance.RootResolver
            .DidNotReceive()
            .Sort(Arg.Any<IEnumerable<BlogSorters>>());
        sutInstance.QueryHandler
            .DidNotReceive()
            .Apply(Arg.Any<IQueryable<Blog>>(), Arg.Any<string>());
        sutInstance.RootResolver
            .DidNotReceive()
            .Page(Arg.Any<PagingOptions>());
        Assert.Equal(sutInstance.Queryable, resultingQuery);
    }

    [Fact]
    public void Should_Apply_QueryLanguage_Filter_Between_Other_Operations()
    {
        var sutInstance = new SUT();
        var filters = new BlogFilters(1, string.Empty, null);
        const string query = "Name %% \"Blog\"";

        var filteredQuery = sutInstance.Queryable
            .Where(b => b.Id == 1);
        var queryFilteredQuery = filteredQuery.Where(b => b.Name.Contains("Blog"));

        sutInstance.RootResolver
            .WithQuery(sutInstance.Queryable)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Filter(filters)
            .Returns(sutInstance.RootResolver);
        sutInstance.RootResolver
            .Apply()
            .Returns(filteredQuery, queryFilteredQuery);
        sutInstance.RootResolver
            .WithQuery(queryFilteredQuery)
            .Returns(sutInstance.RootResolver);

        sutInstance.QueryHandler
            .Apply(filteredQuery, query)
            .Returns(queryFilteredQuery);

        var resultingQuery = sutInstance.Resolver
            .WithQuery(sutInstance.Queryable)
            .Filter(filters)
            .FilterWithQueryLanguage(query)
            .Apply();

        sutInstance.RootResolver
            .Received(1)
            .Filter(filters);
        sutInstance.QueryHandler
            .Received(1)
            .Apply(filteredQuery, query);
        Assert.Equal(queryFilteredQuery, resultingQuery);
    }

    private class SUT
    {
        public IQueryable<Blog> Queryable { get; }
        public IQueryLanguageOperationResolver<Blog, BlogFilters, BlogSorters> Resolver { get; }
        public IOperationResolver<Blog, BlogFilters, BlogSorters> RootResolver { get; }
        public IQueryLanguageHandler<Blog> QueryHandler { get; }
        private ILogger<QueryLanguageOperationResolver<Blog, BlogFilters, BlogSorters>> Log { get; }

        public SUT()
        {
            RootResolver = Substitute.For<IOperationResolver<Blog, BlogFilters, BlogSorters>>();
            QueryHandler = Substitute.For<IQueryLanguageHandler<Blog>>();
            Log = Substitute.For<ILogger<QueryLanguageOperationResolver<Blog, BlogFilters, BlogSorters>>>();

            var startDate = new DateTime(2024, 1, 1);
            var blogFaker = new Faker<Blog>()
                .RuleFor(b => b.Id, f => f.IndexFaker)
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .RuleFor(b => b.NumberOfComments, f => f.Random.Int(0, 100))
                .RuleFor(b => b.CreatedAt, (f, b) => startDate.AddDays(b.Id));
            var blogs = blogFaker.Generate(50);

            Queryable = blogs.AsQueryable();
            Resolver = new QueryLanguageOperationResolver<Blog, BlogFilters, BlogSorters>(
                RootResolver,
                QueryHandler,
                Log);
        }
    }
}