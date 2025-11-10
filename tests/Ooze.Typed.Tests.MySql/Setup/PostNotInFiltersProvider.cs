using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.MySql.Setup;

public class PostNotInFiltersProvider : IFilterProvider<Post, PostInFilters>
{
    public IEnumerable<FilterDefinition<Post, PostInFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostInFilters>()
            .NotIn(post => post.Id, filter => filter.Ids)
            .NotIn(post => post.Name, filter => filter.Names)
            .NotIn(post => post.Date, filter => filter.Dates)
            .Build();
}