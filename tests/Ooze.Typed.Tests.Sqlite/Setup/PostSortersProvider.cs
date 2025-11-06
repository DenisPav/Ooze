using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests.Sqlite.Setup;

public class PostSortersProvider : ISorterProvider<Post, PostSorters>
{
    public IEnumerable<SortDefinition<Post, PostSorters>> GetSorters()
        => Sorters.Sorters.CreateFor<Post, PostSorters>()
            .SortBy(post => post.Id, sort => sort.Id)
            .SortBy(post => post.Enabled, sort => sort.Enabled)
            .Build();
}