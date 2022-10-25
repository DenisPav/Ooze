using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests;

public class SorterBuilderTests
{
    [Fact]
    public void Should_Have_Empty_Sorters_If_None_Are_Added()
    {
        var sorters = Sorters.Sorters.CreateFor<Blog>();

        Assert.Empty(sorters.Build());
    }

    [Fact]
    public void Should_Have_Correct_Number_Of_Sorters()
    {
        var sorters = Sorters.Sorters.CreateFor<Blog>()
            .Add(blog => blog.Id)
            .Add(blog => blog.Name)
            .Add(blog => blog.NumberOfComments)
            .Build();

        Assert.True(sorters.Count() == 3);
    }
}

