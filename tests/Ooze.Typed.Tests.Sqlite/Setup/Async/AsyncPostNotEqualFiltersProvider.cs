using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.Sqlite.Setup.Async;

public class AsyncPostNotEqualFiltersProvider : IAsyncFilterProvider<Post, PostFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostFilters>()
            .NotEqual(post => post.Id, filter => filter.Id)
            .NotEqual(post => post.Name, filter => filter.Name)
            .NotEqual(post => post.Enabled, filter => filter.Enabled)
            .NotEqual(post => post.Date, filter => filter.Date)
            .Build());
}