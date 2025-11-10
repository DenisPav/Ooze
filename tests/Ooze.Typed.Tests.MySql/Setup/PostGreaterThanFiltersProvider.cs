using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.MySql.Setup;

public class PostGreaterThanFiltersProvider : IFilterProvider<Post, PostFilters>
{
    public IEnumerable<FilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .GreaterThan(post => post.Id, filter => filter.Id)
            .GreaterThan(post => post.Name, filter => filter.Name)
            .GreaterThan(post => post.Enabled, filter => filter.Enabled)
            .GreaterThan(post => post.Date, filter => filter.Date)
            .Build();
}