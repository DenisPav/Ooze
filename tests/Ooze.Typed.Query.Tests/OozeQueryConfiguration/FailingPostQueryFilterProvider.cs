using Ooze.Typed.Query.Filters;
using Ooze.Typed.Query.Tests.Entities;

namespace Ooze.Typed.Query.Tests.OozeQueryConfiguration;

public class FailingPostQueryFilterProvider : IQueryLanguageFilterProvider<Post>
{
    public IEnumerable<QueryLanguageFilterDefinition<Post>> GetMappings()
    {
        return QueryLanguageFilters.CreateFor<Post>()
            .Add(x => x.Comments)
            .Build();
    }
}