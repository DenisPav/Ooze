using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.Sqlite.Setup.Async;

public class AsyncPostLessThanFiltersProvider : IAsyncFilterProvider<Post, PostFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostFilters>()
            .LessThan(post => post.Id, filter => filter.Id)
            .LessThan(post => post.Name, filter => filter.Name)
            .LessThan(post => post.Enabled, filter => filter.Enabled)
            .LessThan(post => post.Date, filter => filter.Date)
            .Build());
}