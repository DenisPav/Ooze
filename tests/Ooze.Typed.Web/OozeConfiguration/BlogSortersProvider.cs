using Ooze.Typed.Sorters;

public class BlogSortersProvider : IOozeSorterProvider<Blog, BlogSorters>
{
    public IEnumerable<ISortDefinition<Blog, BlogSorters>> GetSorters()
    {
        return Sorters.CreateFor<Blog, BlogSorters>()
            .Add(blog => blog.Id, sort => sort.BlogIdSort)
            .Add(blog => blog.Name, sort => sort.BlogNameSort)
            .Build();
    }
}
