using Ooze.Typed.EntityFrameworkCore.MySql.Extensions;
using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.MySql.OozeConfiguration;

public class PostFiltersProvider : IFilterProvider<Post, PostFilters>
{
    public IEnumerable<FilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .IsDateDiffDay(post => post.Date, filter => filter.DateDiffDayFilter, DateDiffOperation.Equal)
            .IsDateDiffMonth(post => post.Date, filter => filter.DateDiffMonthFilter, DateDiffOperation.Equal)
            .IsDateDiffYear(post => post.Date, filter => filter.DateDiffYearFilter, DateDiffOperation.Equal)
            .IsDateDiffHour(post => post.Date, filter => filter.DateDiffHourFilter, DateDiffOperation.Equal)
            .IsDateDiffMinute(post => post.Date, filter => filter.DateDiffMinuteFilter, DateDiffOperation.Equal)
            .IsDateDiffSecond(post => post.Date, filter => filter.DateDiffSecondFilter, DateDiffOperation.Equal)
            .IsDateDiffMicrosecond(post => post.Date, filter => filter.DateDiffMicrosecondFilter,
                DateDiffOperation.Equal)
            .Build();
}