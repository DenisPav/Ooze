using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostSortersProvider : IOozeSorterProvider<Post>
{
    public IEnumerable<ISortDefinition<Post>> GetSorters()
        => Sorters.Sorters.CreateFor<Post>()
            .Add(post => post.Id)
            .Add(post => post.Enabled)
            .Build();
}