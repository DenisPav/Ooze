using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.MySql.Setup;

public class PostOutOfRangeFiltersProvider : IFilterProvider<Post, PostRangeFilters>
{
    public IEnumerable<FilterDefinition<Post, PostRangeFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostRangeFilters>()
            .OutOfRange(post => post.Id, filter => filter.Ids)
            .OutOfRange(post => post.Name, filter => filter.Names)
            .OutOfRange(post => post.Date, filter => filter.Dates)
            .Build();
}