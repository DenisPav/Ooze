using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.MySql.Setup.Async;

public class AsyncPostInFiltersProvider : IAsyncFilterProvider<Post, PostInFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostInFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostInFilters>()
            .In(post => post.Id, filter => filter.Ids)
            .In(post => post.Name, filter => filter.Names)
            .In(post => post.Date.Date, filter => filter.Dates)
            .Build());
}