using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.MySql.Setup.Async;

public class AsyncPostNotInFiltersProvider : IAsyncFilterProvider<Post, PostInFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostInFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostInFilters>()
            .NotIn(post => post.Id, filter => filter.Ids)
            .NotIn(post => post.Name, filter => filter.Names)
            .NotIn(post => post.Date, filter => filter.Dates)
            .Build());
}