using Ooze.Typed.EntityFrameworkCore.SqlServer.Extensions;
using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.SqlServer.OozeConfiguration;

public class PostFiltersProvider : IOozeFilterProvider<Post, PostFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .IsDate(post => post.Name, filter => filter.IsNameDate)
            .IsNumeric(post => post.Name, filter => filter.IsIdNumeric)
            .Build();
}