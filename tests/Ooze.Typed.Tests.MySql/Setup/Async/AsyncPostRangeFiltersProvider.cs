using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.MySql.Setup.Async;

public class AsyncPostRangeFiltersProvider : IAsyncFilterProvider<Post, PostRangeFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostRangeFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostRangeFilters>()
            .Range(post => post.Id, filter => filter.Ids)
            .Range(post => post.Name, filter => filter.Names)
            .Range(post => post.Date, filter => filter.Dates)
            .Build());
}