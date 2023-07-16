using Ooze.Typed.EntityFrameworkCore.Npgsql.Extensions;
using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Npgsql.OozeConfiguration;

public class PostFiltersProvider : IOozeFilterProvider<Post, PostFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .InsensitiveLike(post => post.Name, filter => filter.NameLikeFilter)
            .Build();
}