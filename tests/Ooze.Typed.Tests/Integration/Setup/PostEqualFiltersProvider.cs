using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostEqualFiltersProvider : IFilterProvider<Post, PostFilters>
{
    public IEnumerable<FilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .Equal(post => post.Id, filter => filter.Id)
            .Equal(post => post.Name, filter => filter.Name)
            .Equal(post => post.Enabled, filter => filter.Enabled)
            .Equal(post => post.Date, filter => filter.Date)
            .Build();
}