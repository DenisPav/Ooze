using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.Sqlite.Setup.Async;

public class AsyncPostGreaterThanFiltersProvider : IAsyncFilterProvider<Post, PostFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostFilters>()
            .GreaterThan(post => post.Id, filter => filter.Id)
            .GreaterThan(post => post.Name, filter => filter.Name)
            .GreaterThan(post => post.Enabled, filter => filter.Enabled)
            .GreaterThan(post => post.Date, filter => filter.Date)
            .Build());
}