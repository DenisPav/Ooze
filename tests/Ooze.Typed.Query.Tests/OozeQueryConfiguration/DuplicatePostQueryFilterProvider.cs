using Ooze.Typed.Query.Filters;
using Ooze.Typed.Query.Tests.Entities;

namespace Ooze.Typed.Query.Tests.OozeQueryConfiguration;

public class DuplicatePostQueryFilterProvider : IQueryLanguageFilterProvider<Post>
{
    public IEnumerable<QueryLanguageFilterDefinition<Post>> GetMappings()
    {
        return QueryLanguageFilters.CreateFor<Post>()
            .Add(x => x.Id)
            .Add(x => x.GuidId, nameof(Post.Id))
            .Build();
    }
}