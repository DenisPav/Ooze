using Ooze.Typed.Query.Filters;

namespace Ooze.Typed.Web.OozeConfiguration;

public class BlogQueryFiltersProvider : IOozeQueryFilterProvider<Blog>
{
    public IEnumerable<IQueryFilterDefinition<Blog>> GetFilters()
    {
        return QueryFilters.CreateFor<Blog>()
            .Add(x => x.Id, name: "Id")
            .Add(x => x.Name, name: "Name")
            .Build();
    }
}