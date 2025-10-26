using Bogus;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ooze.Typed.Paging;

namespace Ooze.Typed.Tests;

public class PagingHandlerTests
{
    [Fact]
    public void Should_Apply_Offset_And_Limit_With_PagingOptions()
    {
        var sutInstance = new SUT();

        var pagingOptions = new PagingOptions { Page = 1, Size = 5 };
        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Equal(pagingOptions.Size, resultingQuery.Count());
        Assert.Equal(pagingOptions.Size, resultingQuery.First().Id);
    }

    [Fact]
    public void Should_Return_Correct_Page_When_Page_Is_Zero()
    {
        var sutInstance = new SUT();

        var pagingOptions = new PagingOptions { Page = 0, Size = 3 };
        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Equal(pagingOptions.Size, resultingQuery.Count());
        Assert.Equal(0, resultingQuery.First().Id);
        Assert.Equal(2, resultingQuery.Last().Id);
    }

    [Fact]
    public void Should_Return_Correct_Page_When_Page_Is_Greater_Than_Zero()
    {
        var sutInstance = new SUT();

        var pagingOptions = new PagingOptions { Page = 2, Size = 3 };
        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Equal(pagingOptions.Size, resultingQuery.Count());
        Assert.Equal(6, resultingQuery.First().Id);
        Assert.Equal(8, resultingQuery.Last().Id);
    }

    [Fact]
    public void Should_Return_Empty_When_Skip_Exceeds_Query_Length()
    {
        var sutInstance = new SUT();

        var pagingOptions = new PagingOptions { Page = 10, Size = 10 };
        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Empty(resultingQuery);
    }

    [Fact]
    public void Should_Apply_Correct_Size_When_Size_Is_Less_Than_Available()
    {
        var sutInstance = new SUT();

        var pagingOptions = new PagingOptions { Page = 0, Size = 2 };
        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Equal(pagingOptions.Size, resultingQuery.Count());
    }

    [Fact]
    public void Should_Apply_Cursor_Paging_With_Integer_Property()
    {
        var sutInstance = new SUT();

        var pagingOptions = new CursorPagingOptions<int> { After = 5, Size = 3 };
        var resultingQuery = sutInstance.Handler.ApplyCursor(sutInstance.Query, b => b.Id, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Equal(3, resultingQuery.Count());
        Assert.Equal(6, resultingQuery.First().Id);
        Assert.Equal(8, resultingQuery.Last().Id);
    }

    [Fact]
    public void Should_Apply_Cursor_Paging_With_DateTime_Property()
    {
        var sutInstance = new SUT();
        var targetDate = new DateTime(2024, 1, 5);

        var pagingOptions = new CursorPagingOptions<DateTime> { After = targetDate, Size = 3 };
        var resultingQuery = sutInstance.Handler.ApplyCursor(sutInstance.Query, b => b.CreatedAt, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Equal(3, resultingQuery.Count());
        Assert.All(resultingQuery, b => Assert.True(b.CreatedAt > targetDate));
    }

    [Fact]
    public void Should_Filter_Items_Greater_Than_Cursor()
    {
        var sutInstance = new SUT();

        var pagingOptions = new CursorPagingOptions<int> { After = 3, Size = 10 };
        var resultingQuery = sutInstance.Handler.ApplyCursor(sutInstance.Query, b => b.Id, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.All(resultingQuery, b => Assert.True(b.Id > 3));
        Assert.Equal(4, resultingQuery.First().Id);
    }

    [Fact]
    public void Should_Apply_Correct_Size_With_Cursor_Paging()
    {
        var sutInstance = new SUT();

        var pagingOptions = new CursorPagingOptions<int> { After = 0, Size = 5 };
        var resultingQuery = sutInstance.Handler.ApplyCursor(sutInstance.Query, b => b.Id, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Equal(5, resultingQuery.Count());
    }

    [Fact]
    public void Should_Return_Empty_When_No_Items_After_Cursor()
    {
        var sutInstance = new SUT();

        var pagingOptions = new CursorPagingOptions<int> { After = 100, Size = 10 };
        var resultingQuery = sutInstance.Handler.ApplyCursor(sutInstance.Query, b => b.Id, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Empty(resultingQuery);
    }

    [Fact]
    public void Should_Return_Remaining_Items_When_Size_Exceeds_Available()
    {
        var sutInstance = new SUT();

        var pagingOptions = new PagingOptions { Page = 0, Size = 100 };
        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, pagingOptions);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
        Assert.Equal(50, resultingQuery.Count());
    }

    private class SUT
    {
        public IQueryable<Blog> Query { get; }
        public IPagingHandler<Blog> Handler { get; }
        private ILogger<PagingHandler<Blog>> Log { get; }

        public SUT()
        {
            Log = Substitute.For<ILogger<PagingHandler<Blog>>>();

            var startDate = new DateTime(2024, 1, 1);
            var blogFaker = new Faker<Blog>()
                .RuleFor(b => b.Id, f => f.IndexFaker)
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .RuleFor(b => b.CreatedAt, (f, b) => startDate.AddDays(b.Id));

            var blogs = blogFaker.Generate(50);

            Query = blogs.AsQueryable();
            Handler = new PagingHandler<Blog>(Log);
        }
    }
}