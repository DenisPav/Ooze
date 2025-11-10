using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.MySql.Setup;

public class PostInFiltersProvider : IFilterProvider<Post, PostInFilters>
{
    public IEnumerable<FilterDefinition<Post, PostInFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostInFilters>()
            .In(post => post.Id, filter => filter.Ids)
            .In(post => post.Name, filter => filter.Names)
            .In(post => post.Date, filter => filter.Dates)
            .Build();
}