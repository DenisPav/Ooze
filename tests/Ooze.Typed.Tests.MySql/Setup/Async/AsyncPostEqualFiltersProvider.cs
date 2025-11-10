using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.MySql.Setup.Async;

public class AsyncPostEqualFiltersProvider : IAsyncFilterProvider<Post, PostFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostFilters>()
            .Equal(post => post.Id, filter => filter.Id)
            .Equal(post => post.Name, filter => filter.Name)
            .Equal(post => post.Enabled, filter => filter.Enabled)
            .Equal(post => post.Date.Date, filter => filter.Date)
            .Build());
}