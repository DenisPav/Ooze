using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostLessThanFiltersProvider : IFilterProvider<Post, PostFilters>
{
    public IEnumerable<FilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .LessThan(post => post.Id, filter => filter.Id)
            .LessThan(post => post.Name, filter => filter.Name)
            .LessThan(post => post.Enabled, filter => filter.Enabled)
            .LessThan(post => post.Date, filter => filter.Date)
            .Build();
}