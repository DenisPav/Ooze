using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.MySql.Setup;

public class PostNotEqualFiltersProvider : IFilterProvider<Post, PostFilters>
{
    public IEnumerable<FilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .NotEqual(post => post.Id, filter => filter.Id)
            .NotEqual(post => post.Name, filter => filter.Name)
            .NotEqual(post => post.Enabled, filter => filter.Enabled)
            .NotEqual(post => post.Date.Date, filter => filter.Date)
            .Build();
}