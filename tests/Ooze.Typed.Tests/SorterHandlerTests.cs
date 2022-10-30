using System.Linq.Expressions;
using NSubstitute;
using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests;


public class SorterHandlerTests
{
    [Fact]
    public void Should_Have_Same_Query_When_No_Sorters_Are_Present()
    {
        var sutInstance = new SUT();

        sutInstance.Provider1.GetSorters().Returns(Enumerable.Empty<SortDefinition<Blog>>());
        sutInstance.Provider2.GetSorters().Returns(Enumerable.Empty<SortDefinition<Blog>>());
        
        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, new []{ new Sorter("fake-sorter", SortDirection.Ascending) });

        sutInstance.Provider1.Received(1).GetSorters();
        sutInstance.Provider2.Received(1).GetSorters();
        Assert.Equal(sutInstance.Query, resultingQuery);
    }

    [Fact]
    public void Should_Update_Query_When_Sorters_Are_Present()
    {
        var sutInstance = new SUT();

        var sorterDefinition = Substitute.For<SortDefinition<Blog>>();
        Expression<Func<Blog, int>> sorterExpr = x => x.Id;
        sorterDefinition.DataExpression = sorterExpr;
        sorterDefinition.PropertyName = "Id";
        
        sutInstance.Provider1.GetSorters().Returns(new []{ sorterDefinition });
        sutInstance.Provider2.GetSorters().Returns(Enumerable.Empty<SortDefinition<Blog>>());
        
        Assert.Equal(typeof(EnumerableQuery<Blog>), sutInstance.Query.Expression.Type);
        var resultingQuery = sutInstance.Handler.Apply(sutInstance.Query, new []{ new Sorter("Id", SortDirection.Ascending) });

        sutInstance.Provider1.Received(1).GetSorters();
        sutInstance.Provider2.Received(1).GetSorters();
        Assert.False(sutInstance.Query == resultingQuery);
        Assert.Equal(typeof(IOrderedQueryable<Blog>), resultingQuery.Expression.Type);
    }
    
    class SUT
    {
        public IOozeSorterProvider<Blog> Provider1 { get; }
        public IOozeSorterProvider<Blog> Provider2 { get; }
        public IQueryable<Blog> Query { get; }
        public IOozeSorterHandler<Blog> Handler { get; }

        public SUT()
        {
            Provider1 = Substitute.For<IOozeSorterProvider<Blog>>();
            Provider2 = Substitute.For<IOozeSorterProvider<Blog>>();

            Query = Enumerable.Empty<Blog>().AsQueryable();
            Handler = new OozeSorterHandler<Blog>(new[] { Provider1, Provider2 });
        }
    }
}