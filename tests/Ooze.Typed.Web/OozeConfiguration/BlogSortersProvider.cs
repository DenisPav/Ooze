using Ooze.Typed.Sorters;

public class BlogSortersProvider : IOozeSorterProvider<Blog>
{
    public IEnumerable<ISortDefinition<Blog>> GetSorters()
    {
        return Sorters.CreateFor<Blog>()
            .Add(blog => blog.Id)
            .Add(blog => blog.Name)
            .Build();
    }
}
