using Ooze.Typed.EntityFrameworkCore.SqlServer.Extensions;
using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.SqlServer.OozeConfiguration;

public class PostFiltersProvider : IOozeFilterProvider<Post, PostFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .Equal(post => post.Id, filter => filter.PostId)
            .NotEqual(post => post.Id, filter => filter.NotEqualPostId)
            .GreaterThan(post => post.Id, filter => filter.GreaterThanPostId)
            .LessThan(post => post.Id, filter => filter.LessThanPostId)
            .In(post => post.Id, filter => filter.IdIn)
            .NotIn(post => post.Id, filter => filter.IdNotIn)
            .Range(post => post.Id, filter => filter.IdRange)
            .OutOfRange(post => post.Id, filter => filter.IdOutOfRange)
            .StartsWith(post => post.Name, filter => filter.NameStartsWith)
            .DoesntStartWith(post => post.Name, filter => filter.NameDoesntWith)
            .EndsWith(post => post.Name, filter => filter.NameEndsWith)
            .DoesntEndWith(post => post.Name, filter => filter.NameDoesntEndWith)
            .IsDate(post => post.Name, filter => filter.IsNameDate)
            .IsNumeric(post => post.Name, filter => filter.IsIdNumeric)
            .IsDateDiffDay(post => post.Date, filter => filter.DateDiffDay, DateDiffOperation.NotEqual)
            .IsDateDiffDay(post => post.Date, filter => filter.DateDiffDayEqual, DateDiffOperation.Equal)
            .IsDateDiffMonth(post => post.Date, filter => filter.DateDiffMonth, DateDiffOperation.NotEqual)
            .IsDateDiffYear(post => post.Date, filter => filter.DateDiffYear, DateDiffOperation.NotEqual)
            .IsDateDiffWeek(post => post.Date, filter => filter.DateDiffWeek, DateDiffOperation.NotEqual)
            .IsDateDiffHour(post => post.Date, filter => filter.DateDiffHour, DateDiffOperation.NotEqual)
            .IsDateDiffMinute(post => post.Date, filter => filter.DateDiffMinute, DateDiffOperation.NotEqual)
            .IsDateDiffSecond(post => post.Date, filter => filter.DateDiffSecond, DateDiffOperation.NotEqual)
            .IsDateDiffMillisecond(post => post.Date, filter => filter.DateDiffMillisecond, DateDiffOperation.NotEqual)
            .IsDateDiffMicrosecond(post => post.Date, filter => filter.DateDiffMicrosecond, DateDiffOperation.NotEqual)
            .IsDateDiffNanosecond(post => post.Date, filter => filter.DateDiffNanosecond, DateDiffOperation.NotEqual)
            .Build();
}