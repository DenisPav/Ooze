using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests;


public class SorterHandlerTests
{
    [Fact]
    public void Should_Have_Same_Query_When_No_Sorters_Are_Present()
    {
        var sutInstance = new SUT();

        sutInstance.Provider1.GetSorters().Returns(Enumerable.Empty<SortDefinition<Blog, BlogSorters>>());
        sutInstance.Provider2.GetSorters().Returns(Enumerable.Empty<SortDefinition<Blog, BlogSorters>>());

        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, Enumerable.Empty<BlogSorters>());

        sutInstance.Provider1.Received(1).GetSorters();
        sutInstance.Provider2.Received(1).GetSorters();
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Update_Query_When_Sorters_Are_Present()
    {
        var sutInstance = new SUT();

        var sorterDefinition = Substitute.For<SortDefinition<Blog, BlogSorters>>();
        Expression<Func<Blog, int>> sorterExpr = x => x.Id;
        sorterDefinition.ShouldRun = _ => true;
        sorterDefinition.GetSortDirection = _ => SortDirection.Ascending;
        sorterDefinition.DataExpression = sorterExpr;

        sutInstance.Provider1.GetSorters().Returns(new[] { sorterDefinition });
        sutInstance.Provider2.GetSorters().Returns(Enumerable.Empty<SortDefinition<Blog, BlogSorters>>());

        Assert.Equal(typeof(EnumerableQuery<Blog>), sutInstance.Query.Expression.Type);
        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, new[] { new BlogSorters(SortDirection.Ascending, null, null) });

        sutInstance.Provider1.Received(1).GetSorters();
        sutInstance.Provider2.Received(1).GetSorters();
        Assert.False(sutInstance.Query == resultingQuery);
        Assert.Equal(typeof(IOrderedQueryable<Blog>), resultingQuery.Expression.Type);
    }

    class SUT
    {
        public ISorterProvider<Blog, BlogSorters> Provider1 { get; }
        public ISorterProvider<Blog, BlogSorters> Provider2 { get; }
        public IQueryable<Blog> Query { get; }
        public ISorterHandler<Blog, BlogSorters> Handler { get; }
        private ILogger<SorterHandler<Blog, BlogSorters>> Log { get; }

        public SUT()
        {
            Provider1 = Substitute.For<ISorterProvider<Blog, BlogSorters>>();
            Provider2 = Substitute.For<ISorterProvider<Blog, BlogSorters>>();
            Log = Substitute.For<ILogger<SorterHandler<Blog, BlogSorters>>>();

            Query = Enumerable.Empty<Blog>().AsQueryable();
            Handler = new SorterHandler<Blog, BlogSorters>(new[] { Provider1, Provider2 }, Log);
        }
    }
}