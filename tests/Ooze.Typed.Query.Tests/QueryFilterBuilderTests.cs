using Ooze.Typed.Query.Exceptions;
using Ooze.Typed.Query.Filters;

namespace Ooze.Typed.Query.Tests;

public class QueryFilterBuilderTests
{
    [Fact]
    public void Should_Throw_If_Expression_Is_Not_Member_Expression()
    {
        var filterBuilder = new QueryFilterBuilder<Blog>();
        Assert.Throws<MemberExpressionException>(() => filterBuilder.Add(x => x));
    }
    
    [Fact]
    public void Should_Throw_If_Expression_Is_Not_Member_Expression_With_Custom_Name_Provided()
    {
        var filterBuilder = new QueryFilterBuilder<Blog>();
        Assert.Throws<MemberExpressionException>(() => filterBuilder.Add(x => x, "NewName"));
    }
    
    [Fact]
    public void Should_Return_Correct_Number_Of_Definitions()
    {
        var filterDefinitions = new QueryFilterBuilder<Blog>()
            .Add(x => x.Id)
            .Add(x => x.Name, "CustomName")
            .Add(x => x.Date)
            .Build()
            .Cast<QueryFilterDefinition<Blog>>();

        var expectedNames = new[] { "Id", "CustomName", "Date" };
        Assert.True(filterDefinitions.Count() == 3);
        Assert.True(filterDefinitions.All(x => expectedNames.Contains(x.Name)));
    }
}