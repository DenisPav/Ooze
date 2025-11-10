using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.MySql.Setup.Async;

public class AsyncPostOutOfRangeFiltersProvider : IAsyncFilterProvider<Post, PostRangeFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostRangeFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostRangeFilters>()
            .OutOfRange(post => post.Id, filter => filter.Ids)
            .OutOfRange(post => post.Name, filter => filter.Names)
            .OutOfRange(post => post.Date, filter => filter.Dates)
            .Build());
}