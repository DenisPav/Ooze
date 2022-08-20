using Ooze.Typed.Filters;
using System.Linq.Expressions;

namespace Ooze.Typed.Tests;

public class FilterBuilderTests
{
    [Fact]
    public void Should_Have_Empty_Filters_If_None_Are_Added()
    {
        var filters = Filters.Filters.CreateFor<Blog, BlogFilters>();

        Assert.Empty(filters.Build());
    }

    [Fact]
    public void Should_Have_Correct_Number_Of_Filters()
    {
        var filters = Filters.Filters.CreateFor<Blog, BlogFilters>()
            .Equal(blog => blog.Id, filter => filter.IdFilter)
            .Equal(blog => blog.Name, filter => filter.NameFilter)
            .NotEqual(blog => blog.NumberOfComments, filter => filter.NumberOfCommentsFilter)
            .Build();

        Assert.True(filters.Count() == 3);
    }

    [Fact]
    public void Should_Throw_Exception_When_Not_Targeting_Member()
    {
        var filters = Filters.Filters.CreateFor<Blog, BlogFilters>()
            .Equal(blog => blog.Name.ToString(), filter => filter.NameFilter)
            .Build();

        Assert.True(filters.Count() == 1);

        var filter = filters.Cast<FilterDefinition<Blog, BlogFilters>>()
            .Single();
        Assert.Throws<Exception>(() => filter.FilterExpressionFactory(new(1, string.Empty, 1)));
    }

    [Fact]
    public void Should_Contain_Custom_Filter()
    {
        var filters = Filters.Filters.CreateFor<Blog, BlogFilters>()
            .Add(
                filters => filters.IdFilter.HasValue,
                filters =>
                {
                    var id = filters.IdFilter.GetValueOrDefault();
                    return blog => blog.Id >= id;
                })
            .Build();

        Assert.True(filters.Count() == 1);
    }

    [Theory]
    [InlineData(null, null, null, 0)]
    [InlineData(1, null, null, 1)]
    [InlineData(1, "name", null, 2)]
    [InlineData(1, "name", 2, 3)]
    [InlineData(null, "name", -3, 2)]
    [InlineData(null, null, 32, 1)]
    [InlineData(null, "name", null, 1)]
    public void Should_Run_Correct_Filters(
        int? id,
        string name,
        int? numberOfComments,
        int numberOfFilters)
    {
        var filters = Filters.Filters.CreateFor<Blog, BlogFilters>()
            .Equal(blog => blog.Id, filter => filter.IdFilter)
            .Equal(blog => blog.Name, filter => filter.NameFilter)
            .NotEqual(blog => blog.NumberOfComments, filter => filter.NumberOfCommentsFilter)
            .Build()
            .Cast<FilterDefinition<Blog, BlogFilters>>();

        var filterPayload = new BlogFilters(id, name, numberOfComments);
        var totalNumberOfRunnableFilters = filters.Count(filter => filter.ShouldRun(filterPayload));

        Assert.Equal(totalNumberOfRunnableFilters, numberOfFilters);
    }

    [Theory]
    [InlineData(null, null, null, 0)]
    [InlineData(1, null, null, 1)]
    [InlineData(1, "name", null, 1)]
    [InlineData(1, "name", 2, 1)]
    [InlineData(null, "name", -3, 0)]
    [InlineData(null, null, 32, 1)]
    [InlineData(3213, "123", 4, 3)]
    public void Should_Run_Correct_Custom_Filters(
        int? id,
        string name,
        int? numberOfComments,
        int numberOfFilters)
    {
        Func<BlogFilters, Expression<Func<Blog, bool>>> fakeFilterHandler = _ => blog => true;

        var filters = Filters.Filters.CreateFor<Blog, BlogFilters>()
            .Add(filter => filter.IdFilter.HasValue, fakeFilterHandler)
            .Add(filter => string.IsNullOrEmpty(filter.NameFilter) == false 
                && filter.NameFilter.Contains("123"), fakeFilterHandler)
            .Add(filter => filter.NumberOfCommentsFilter > 3, fakeFilterHandler)
            .Build()
            .Cast<FilterDefinition<Blog, BlogFilters>>();

        var filterPayload = new BlogFilters(id, name, numberOfComments);
        var totalNumberOfRunnableFilters = filters.Count(filter => filter.ShouldRun(filterPayload));

        Assert.Equal(totalNumberOfRunnableFilters, numberOfFilters);
    }
}

