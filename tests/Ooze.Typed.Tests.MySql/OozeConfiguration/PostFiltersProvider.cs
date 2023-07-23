using Ooze.Typed.EntityFrameworkCore.MySql.Extensions;
using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.MySql.OozeConfiguration;

public class PostFiltersProvider : IOozeFilterProvider<Post, PostFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .IsDateDiffDay(post => post.Date, filter => filter.DateFilter, DateDiffOperation.Equal)
            .Build();
}