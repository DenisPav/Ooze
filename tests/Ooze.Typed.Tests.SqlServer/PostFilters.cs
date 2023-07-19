using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.SqlServer;

public record PostFilters(
    long? PostId = null,
    long? NotEqualPostId = null,
    long? GreaterThanPostId = null,
    long? LessThanPostId = null,
    IEnumerable<long>? IdIn = null,
    IEnumerable<long>? IdNotIn = null,
    RangeFilter<long>? IdRange = null,
    RangeFilter<long>? IdOutOfRange = null,
    string? NameStartsWith = null,
    string? NameDoesntWith = null,
    string? NameEndsWith = null,
    string? NameDoesntEndWith = null,
    bool? IsNameDate = null,
    bool? IsIdNumeric = null,
    DateTime? DateDiffDay = null,
    DateTime? DateDiffMonth = null,
    DateTime? DateDiffYear = null,
    DateTime? DateDiffWeek = null,
    DateTime? DateDiffHour = null,
    DateTime? DateDiffMinute = null,
    DateTime? DateDiffSecond = null,
    DateTime? DateDiffMillisecond = null,
    DateTime? DateDiffDayEqual = null,
    DateTime? DateDiffMicrosecond = null,
    DateTime? DateDiffNanosecond = null
);