using Ooze.Typed.EntityFrameworkCore.Sqlite.Extensions;
using Ooze.Typed.Filters;

public class BlogFiltersProvider : IOozeFilterProvider<Blog, BlogFilters>
{
    public IEnumerable<IFilterDefinition<Blog, BlogFilters>> GetFilters()
    {
        return Filters.CreateFor<Blog, BlogFilters>()
            .Equal(blog => blog.Id, filter => filter.BlogId)
            .Range(blog => blog.Id, filter => filter.BlogRange)
            .In(blog => blog.Id, filter => filter.BlogIds)
            .Glob(blog => blog.Name, filter => filter.Name)
            .Build();
    }
}
