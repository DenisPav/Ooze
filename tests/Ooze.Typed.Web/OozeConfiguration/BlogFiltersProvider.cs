using Ooze.Typed.EntityFrameworkCore.Extensions;
using Ooze.Typed.EntityFrameworkCore.SqlServer.Extensions;
using Ooze.Typed.Filters;
using Ooze.Typed.Web.Entities;

public class BlogFiltersProvider : IOozeFilterProvider<Blog, BlogFilters>
{
    public IEnumerable<IFilterDefinition<Blog, BlogFilters>> GetFilters()
    {
        return Filters.CreateFor<Blog, BlogFilters>()
            .Equal(blog => blog.Id, filter => filter.BlogId)
            .Range(blog => blog.Id, filter => filter.BlogRange)
            .In(blog => blog.Id, filter => filter.BlogIds)
            // .Like(blog => blog.Name, filter => filter.Name)
            .IsDate(blog => blog.Name, filter => filter.IsNameDate)
            .Build();
    }
}
