using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostSortersProvider : IOozeSorterProvider<Post, PostSorters>
{
    public IEnumerable<ISortDefinition<Post, PostSorters>> GetSorters()
        => Sorters.Sorters.CreateFor<Post, PostSorters>()
            .Add(post => post.Id, sort => sort.Id)
            .Build();
}