using Ooze.Typed.Sorters.Async;

namespace Ooze.Typed.Tests.Sqlite.Setup.Async;

public class AsyncPostSortersProvider : IAsyncSorterProvider<Post, PostSorters>
{
    public ValueTask<IEnumerable<AsyncSortDefinition<Post, PostSorters>>> GetSortersAsync()
        => ValueTask.FromResult(AsyncSorters.CreateFor<Post, PostSorters>()
            .SortBy(post => post.Id, sort => sort.Id)
            .SortBy(post => post.Enabled, sort => sort.Enabled)
            .Build());
}