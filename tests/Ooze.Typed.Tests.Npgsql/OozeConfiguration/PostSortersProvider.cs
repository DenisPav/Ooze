using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests.Npgsql.OozeConfiguration;

public class PostSortersProvider : IOozeSorterProvider<Post, PostSorters>
{
    public IEnumerable<ISortDefinition<Post, PostSorters>> GetSorters()
        => Sorters.Sorters.CreateFor<Post, PostSorters>()
            .SortBy(post => post.Id, sort => sort.Id)
            .SortBy(post => post.Enabled, sort => sort.Enabled)
            .Build();
}