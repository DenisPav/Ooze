using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostOutOfRangeFiltersProvider : IOozeFilterProvider<Post, PostRangeFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostRangeFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostRangeFilters>()
            .OutOfRange(post => post.Id, filter => filter.Ids)
            .OutOfRange(post => post.Name, filter => filter.Names)
            .OutOfRange(post => post.Date, filter => filter.Dates)
            .Build();
}