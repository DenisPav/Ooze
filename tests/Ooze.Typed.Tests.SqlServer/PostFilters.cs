namespace Ooze.Typed.Tests.SqlServer;

public record PostFilters(
    bool? IsNameDate,
    bool? IsIdNumeric,
    DateTime? DateDiffDay,
    DateTime? DateDiffMonth,
    DateTime? DateDiffYear,
    DateTime? DateDiffWeek,
    DateTime? DateDiffHour,
    DateTime? DateDiffMinute,
    DateTime? DateDiffSecond,
    DateTime? DateDiffMillisecond,
    DateTime? DateDiffDayEqual,
    DateTime? DateDiffMicrosecond,
    DateTime? DateDiffNanosecond
);