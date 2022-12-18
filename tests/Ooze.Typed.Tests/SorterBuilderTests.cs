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
            .Add(blog => blog.Id, sort => sort.IdSort)
            .Add(blog => blog.Name, sort => sort.NameSort)
            .Add(blog => blog.NumberOfComments, sort => sort.NumberOfCommentsSort)
            .Build();

        Assert.True(sorters.Count() == 3);
    }
}

