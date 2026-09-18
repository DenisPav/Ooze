using Microsoft.Extensions.Logging;
using NSubstitute;
using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests;

public class FilterHandlerTests
{
    [Fact]
    public void Should_Have_Same_Query_When_No_Filters_Are_Present()
    {
        var sutInstance = new SUT();

        sutInstance.Provider1.GetFilters().Returns([]);
        sutInstance.Provider2.GetFilters().Returns([]);

        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, new BlogFilters(1, "dsads", 231));

        sutInstance.Provider1.Received(1).GetFilters();
        sutInstance.Provider2.Received(1).GetFilters();
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Call_ShouldRun_On_Existing_Filters()
    {
        var sutInstance = new SUT();

        var filterDefinition1 = Substitute.For<FilterDefinition<Blog, BlogFilters>>();
        filterDefinition1.FilterExpressionFactory = _ => blog => true;
        filterDefinition1.ShouldRun = _ => true;
        var filterDefinition2 = Substitute.For<FilterDefinition<Blog, BlogFilters>>();
        filterDefinition2.FilterExpressionFactory = _ => blog => false;
        filterDefinition2.ShouldRun = _ => false;

        sutInstance.Provider1.GetFilters().Returns([filterDefinition1, filterDefinition2]);
        sutInstance.Provider2.GetFilters().Returns([]);

        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, new BlogFilters(1, "dsads", 231));
        var fakeFilters = new BlogFilters(null, string.Empty, null);

        sutInstance.Provider1.Received(1).GetFilters();
        sutInstance.Provider2.Received(1).GetFilters();
        filterDefinition1.Received(1).ShouldRun(fakeFilters);
        filterDefinition2.Received(1).ShouldRun(fakeFilters);
        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    [Fact]
    public void Should_Call_Correct_FilterExpressionFactory()
    {
        var sutInstance = new SUT();

        var filterDefinition1 = Substitute.For<FilterDefinition<Blog, BlogFilters>>();
        filterDefinition1.FilterExpressionFactory = _ => blog => true;
        filterDefinition1.ShouldRun = _ => true;
        var filterDefinition2 = Substitute.For<FilterDefinition<Blog, BlogFilters>>();
        filterDefinition2.FilterExpressionFactory = _ => blog => false;
        filterDefinition2.ShouldRun = _ => false;

        sutInstance.Provider1.GetFilters().Returns([filterDefinition1, filterDefinition2]);
        sutInstance.Provider2.GetFilters().Returns([]);

        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, new BlogFilters(1, "dsads", 231));
        var fakeFilters = new BlogFilters(null, string.Empty, null);

        sutInstance.Provider1.Received(1).GetFilters();
        sutInstance.Provider2.Received(1).GetFilters();
        filterDefinition1.Received(1).ShouldRun(fakeFilters);
        filterDefinition2.Received(1).ShouldRun(fakeFilters);
        filterDefinition1.Received(1).FilterExpressionFactory(fakeFilters);
        filterDefinition2.DidNotReceive().FilterExpressionFactory(fakeFilters);

        Assert.False(Equals(sutInstance.Query, resultingQuery));
    }

    private class SUT
    {
        public IFilterProvider<Blog, BlogFilters> Provider1 { get; }
        public IFilterProvider<Blog, BlogFilters> Provider2 { get; }
        public IQueryable<Blog> Query { get; }
        public IFilterHandler<Blog, BlogFilters> Handler { get; }
        private ILogger<FilterHandler<Blog, BlogFilters>> Log { get; }

        public SUT()
        {
            Provider1 = Substitute.For<IFilterProvider<Blog, BlogFilters>>();
            Provider2 = Substitute.For<IFilterProvider<Blog, BlogFilters>>();
            Log = Substitute.For<ILogger<FilterHandler<Blog, BlogFilters>>>();

            Query = Enumerable.Empty<Blog>().AsQueryable();
            Handler = new FilterHandler<Blog, BlogFilters>([Provider1, Provider2], Log);
        }
    }
}