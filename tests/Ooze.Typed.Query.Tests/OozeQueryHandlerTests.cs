using Microsoft.Extensions.Logging;
using NSubstitute;
using Ooze.Typed.Query.Exceptions;
using Ooze.Typed.Query.Filters;

namespace Ooze.Typed.Query.Tests;

public class OozeQueryHandlerTests
{
    [Fact]
    public void Should_Call_GetFilters_On_Filter_Providers()
    {
        var sutInstance = new SUT();
        var filters = QueryFilters.CreateFor<Blog>()
            .Add(x => x.Id)
            .Build();
        sutInstance.QueryFilterProvider1.GetFilters().Returns(filters);
        sutInstance.QueryFilterProvider2.GetFilters().Returns(Enumerable.Empty<IQueryFilterDefinition<Blog>>());

        sutInstance.Handler.Apply(sutInstance.Query, "Id == '3'");
        
        sutInstance.QueryFilterProvider1.Received(1).GetFilters();
        sutInstance.QueryFilterProvider2.Received(1).GetFilters();
    }
    
    [Fact]
    public void Should_Throw_If_Not_Supported_Characters_Are_In_The_Query()
    {
        var sutInstance = new SUT();
        var filters = QueryFilters.CreateFor<Blog>()
            .Add(x => x.Id)
            .Build();
        sutInstance.QueryFilterProvider1.GetFilters().Returns(filters);
        sutInstance.QueryFilterProvider2.GetFilters().Returns(Enumerable.Empty<IQueryFilterDefinition<Blog>>());

        Assert.Throws<QueryTokenizerException>(() => sutInstance.Handler.Apply(sutInstance.Query, "$#!$!%"));
        
        sutInstance.QueryFilterProvider1.Received(1).GetFilters();
        sutInstance.QueryFilterProvider2.Received(1).GetFilters();
    }
    
    [Fact]
    public void Should_Throw_If_Missing_Matching_Starting_Bracket()
    {
        var sutInstance = new SUT();
        var filters = QueryFilters.CreateFor<Blog>()
            .Add(x => x.Id)
            .Build();
        sutInstance.QueryFilterProvider1.GetFilters().Returns(filters);
        sutInstance.QueryFilterProvider2.GetFilters().Returns(Enumerable.Empty<IQueryFilterDefinition<Blog>>());

        Assert.Throws<ExpressionCreatorException>(() => sutInstance.Handler.Apply(sutInstance.Query, "(Id == '3'))"));
        
        sutInstance.QueryFilterProvider1.Received(1).GetFilters();
        sutInstance.QueryFilterProvider2.Received(1).GetFilters();
    }
    
    [Fact]
    public void Should_Throw_If_Missing_Matching_Ending_Bracket()
    {
        var sutInstance = new SUT();
        var filters = QueryFilters.CreateFor<Blog>()
            .Add(x => x.Id)
            .Build();
        sutInstance.QueryFilterProvider1.GetFilters().Returns(filters);
        sutInstance.QueryFilterProvider2.GetFilters().Returns(Enumerable.Empty<IQueryFilterDefinition<Blog>>());

        Assert.Throws<ExpressionCreatorException>(() => sutInstance.Handler.Apply(sutInstance.Query, "((Id == '3')"));
        
        sutInstance.QueryFilterProvider1.Received(1).GetFilters();
        sutInstance.QueryFilterProvider2.Received(1).GetFilters();
    }
    
    [Theory]
    [InlineData("'14' == Id")]
    [InlineData(" == '3' Id")]
    public void Should_Throw_If_Query_Elements_Not_In_Correct_Order(string queryDefinition)
    {
        var sutInstance = new SUT();
        var filters = QueryFilters.CreateFor<Blog>()
            .Add(x => x.Id)
            .Build();
        sutInstance.QueryFilterProvider1.GetFilters().Returns(filters);
        sutInstance.QueryFilterProvider2.GetFilters().Returns(Enumerable.Empty<IQueryFilterDefinition<Blog>>());

        Assert.Throws<ExpressionCreatorException>(() => sutInstance.Handler.Apply(sutInstance.Query, queryDefinition));
        
        sutInstance.QueryFilterProvider1.Received(1).GetFilters();
        sutInstance.QueryFilterProvider2.Received(1).GetFilters();
    }
    
    [Theory]
    [InlineData("Id @= '3'")]
    [InlineData("Id =@ '3'")]
    [InlineData("Id %% '3'")]
    [InlineData("Name >> '3'")]
    [InlineData("Name << '3'")]
    [InlineData("Name >= '3'")]
    [InlineData("Name <= '3'")]
    [InlineData("ExternalId @= '00000000-0000-0000-0000-000000000000'")]
    [InlineData("ExternalId =@ '00000000-0000-0000-0000-000000000000'")]
    [InlineData("ExternalId %% '00000000-0000-0000-0000-000000000000'")]
    [InlineData("Date @= '2022-11-27'")]
    [InlineData("Date =@ '2022-11-27'")]
    [InlineData("Date %% '2022-11-27'")]
    public void Should_Throw_If_Using_Unsupported_Operation(string queryDefinition)
    {
        var sutInstance = new SUT();
        var filters = QueryFilters.CreateFor<Blog>()
            .Add(x => x.Id)
            .Add(x => x.Name)
            .Add(x => x.ExternalId)
            .Add(x => x.Date)
            .Build();
        sutInstance.QueryFilterProvider1.GetFilters().Returns(filters);
        sutInstance.QueryFilterProvider2.GetFilters().Returns(Enumerable.Empty<IQueryFilterDefinition<Blog>>());

        Assert.Throws<ExpressionCreatorException>(() => sutInstance.Handler.Apply(sutInstance.Query, queryDefinition));

        sutInstance.QueryFilterProvider1.Received(1).GetFilters();
        sutInstance.QueryFilterProvider2.Received(1).GetFilters();
    }
    
    [Theory]
    [InlineData("Id == '3'")]
    [InlineData("Id != '3'")]
    [InlineData("Id >> '3'")]
    [InlineData("Id << '3'")]
    [InlineData("Id >= '3'")]
    [InlineData("Id <= '3'")]
    [InlineData("Name == '3'")]
    [InlineData("Name != '3'")]
    [InlineData("Name @= '3'")]
    [InlineData("Name =@ '3'")]
    [InlineData("Name %% '3'")]
    [InlineData("ExternalId == '00000000-0000-0000-0000-000000000000'")]
    [InlineData("ExternalId != '00000000-0000-0000-0000-000000000000'")]
    [InlineData("ExternalId >> '00000000-0000-0000-0000-000000000000'")]
    [InlineData("ExternalId << '00000000-0000-0000-0000-000000000000'")]
    [InlineData("ExternalId >= '00000000-0000-0000-0000-000000000000'")]
    [InlineData("ExternalId <= '00000000-0000-0000-0000-000000000000'")]
    [InlineData("Date == '2022-11-27'")]
    [InlineData("Date != '2022-11-27'")]
    [InlineData("Date >> '2022-11-27'")]
    [InlineData("Date << '2022-11-27'")]
    [InlineData("Date <= '2022-11-27'")]
    [InlineData("Date >= '2022-11-27'")]
    public void Should_Update_Queryable_If_Correct_Operation_Is_Supported_For_Type(string queryDefinition)
    {
        var sutInstance = new SUT();
        var filters = QueryFilters.CreateFor<Blog>()
            .Add(x => x.Id)
            .Add(x => x.Name)
            .Add(x => x.ExternalId)
            .Add(x => x.Date)
            .Build();
        sutInstance.QueryFilterProvider1.GetFilters().Returns(filters);
        sutInstance.QueryFilterProvider2.GetFilters().Returns(Enumerable.Empty<IQueryFilterDefinition<Blog>>());

        var query = sutInstance.Handler.Apply(sutInstance.Query, queryDefinition);
        
        Assert.True(sutInstance.Query != query);
        sutInstance.QueryFilterProvider1.Received(1).GetFilters();
        sutInstance.QueryFilterProvider2.Received(1).GetFilters();
    }

    class SUT
    {
        public IOozeQueryFilterProvider<Blog> QueryFilterProvider1 { get; }
        public IOozeQueryFilterProvider<Blog> QueryFilterProvider2 { get; }
        public IQueryable<Blog> Query { get; }
        public OozeQueryHandler<Blog> Handler { get; }

        public SUT()
        {
            QueryFilterProvider1 = Substitute.For<IOozeQueryFilterProvider<Blog>>();
            QueryFilterProvider2 = Substitute.For<IOozeQueryFilterProvider<Blog>>();
            var log = Substitute.For<ILogger<OozeQueryHandler<Blog>>>();
            Query = Enumerable.Empty<Blog>().AsQueryable();
            
            Handler = new OozeQueryHandler<Blog>(new[] { QueryFilterProvider1, QueryFilterProvider2 }, log);
        }

        
    }
}