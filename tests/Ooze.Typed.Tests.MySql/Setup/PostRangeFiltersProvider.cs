using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.MySql.Setup;

public class PostRangeFiltersProvider : IFilterProvider<Post, PostRangeFilters>
{
    public IEnumerable<FilterDefinition<Post, PostRangeFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostRangeFilters>()
            .Range(post => post.Id, filter => filter.Ids)
            .Range(post => post.Name, filter => filter.Names)
            .Range(post => post.Date, filter => filter.Dates)
            .Build();
}