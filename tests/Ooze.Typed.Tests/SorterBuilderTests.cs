using Ooze.Typed.Sorters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ooze.Typed.Tests;

public class SorterBuilderTests
{
    [Fact]
    public void Should_Have_Empty_Sorters_If_None_Are_Added()
    {
        var sorters = Sorters.Sorters.CreateFor<Blog, BlogSorters>();

        Assert.Empty(sorters.Build());
    }

    [Fact]
    public void Should_Have_Correct_Number_Of_Sorters()
    {
        var sorters = Sorters.Sorters.CreateFor<Blog, BlogSorters>()
            .Add(blog => blog.Id, sorter => sorter.IdSort)
            .Add(blog => blog.Name, sorter => sorter.NameSort)
            .Add(blog => blog.NumberOfComments, sorter => sorter.NumberOfCommentsSort)
            .Build();

        Assert.True(sorters.Count() == 3);
    }

    //TODO: rename Custom filter to Add
    [Theory]
    [InlineData(null, null, null, 0)]
    [InlineData(SortDirection.Ascending, null, null, 1)]
    [InlineData(SortDirection.Ascending, SortDirection.Descending, null, 2)]
    [InlineData(SortDirection.Ascending, SortDirection.Ascending, SortDirection.Ascending, 3)]
    [InlineData(null, SortDirection.Descending, SortDirection.Descending, 2)]
    [InlineData(null, null, SortDirection.Ascending, 1)]
    [InlineData(null, SortDirection.Descending, null, 1)]
    public void Should_Run_Correct_Sorters(
        SortDirection? id,
        SortDirection? name,
        SortDirection? numberOfComments,
        int numberOfSorters)
    {
        var sorters = Sorters.Sorters.CreateFor<Blog, BlogSorters>()
            .Add(blog => blog.Id, sorter => sorter.IdSort)
            .Add(blog => blog.Name, sorter => sorter.NameSort)
            .Add(blog => blog.NumberOfComments, sorter => sorter.NumberOfCommentsSort)
            .Build()
            .Cast<SortDefinition<Blog, BlogSorters>>();

        var sorterPayload = new BlogSorters(id, name, numberOfComments);
        var totalNumberOfRunnableFilters = sorters.Count(sorter => sorter.ShouldRun(sorterPayload));

        Assert.Equal(totalNumberOfRunnableFilters, numberOfSorters);
    }
}

