using Ooze.Typed.EntityFrameworkCore.Npgsql.Extensions;
using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Npgsql.Setup;

public class PostPsqlFiltersProvider : IFilterProvider<Post, PostPsqlFilters>
{
    public IEnumerable<FilterDefinition<Post, PostPsqlFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostPsqlFilters>()
            .InsensitiveLike(post => post.Name, filter => filter.ILikeFilter)
            .SoundexEqual(post => post.Name, filter => filter.SoundexFilter)
            .Build();
}