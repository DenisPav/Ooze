using Ooze.Typed.Query.Filters;
using Ooze.Typed.Query.Tests.Entities;

namespace Ooze.Typed.Query.Tests.OozeQueryConfiguration;

public class CommentQueryFilterProvider : IQueryLanguageFilterProvider<Comment>
{
    public IEnumerable<QueryLanguageFilterDefinition<Comment>> GetMappings()
    {
        return QueryLanguageFilters.CreateFor<Comment>()
            .Add(x => x.User.Email)
            .Build();
    }
}