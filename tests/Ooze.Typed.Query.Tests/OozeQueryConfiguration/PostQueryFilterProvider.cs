using Ooze.Typed.Query.Filters;
using Ooze.Typed.Query.Tests.Entities;

namespace Ooze.Typed.Query.Tests.OozeQueryConfiguration;

public class PostQueryFilterProvider : IQueryLanguageFilterProvider<Post>
{
    public IEnumerable<QueryLanguageFilterDefinition<Post>> GetMappings()
    {
        return QueryLanguageFilters.CreateFor<Post>()
            .Add(x => x.Id)
            .Add(x => x.GuidId)
            .Add(x => x.Date)
            .Add(x => x.OnlyDate)
            .Add(x => x.Enabled)
            .Add(x => x.Name)
            .Add(x => x.Rating)
            .Build();
    }
}