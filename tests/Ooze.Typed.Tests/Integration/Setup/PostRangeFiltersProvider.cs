using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostRangeFiltersProvider : IOozeFilterProvider<Post, PostRangeFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostRangeFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostRangeFilters>()
            .Range(post => post.Id, filter => filter.Ids)
            .Range(post => post.Name, filter => filter.Names)
            .Range(post => post.Date, filter => filter.Dates)
            .Build();
}