using Ooze.Typed.Sorters;
using Ooze.Typed.Sorters.Async;

namespace Ooze.Typed.Tests.MySql.Setup;

public class PostSortersProvider : ISorterProvider<Post, PostSorters>, IAsyncSorterProvider<Post, PostSorters>
{
    public IEnumerable<SortDefinition<Post, PostSorters>> GetSorters()
        => Sorters.Sorters.CreateFor<Post, PostSorters>()
            .SortBy(post => post.Id, sort => sort.Id)
            .SortBy(post => post.Enabled, sort => sort.Enabled)
            .Build();
    
    public ValueTask<IEnumerable<AsyncSortDefinition<Post, PostSorters>>> GetSortersAsync()
        => ValueTask.FromResult(AsyncSorters.CreateFor<Post, PostSorters>()
            .SortBy(post => post.Id, sort => sort.Id)
            .SortBy(post => post.Enabled, sort => sort.Enabled)
            .Build());
}