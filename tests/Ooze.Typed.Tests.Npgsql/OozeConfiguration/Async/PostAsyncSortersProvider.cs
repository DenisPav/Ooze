using Ooze.Typed.Sorters.Async;

namespace Ooze.Typed.Tests.Npgsql.OozeConfiguration.Async;

public class PostAsyncSortersProvider : IAsyncSorterProvider<Post, PostSorters>
{
    public ValueTask<IEnumerable<AsyncSortDefinition<Post, PostSorters>>> GetSortersAsync()
        => ValueTask.FromResult(AsyncSorters.CreateFor<Post, PostSorters>()
            .SortBy(post => post.Id, sort => sort.Id)
            .Build());
}