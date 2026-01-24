namespace Ooze.Typed.Tests.SqlServer.Setup;

public record PostDateFilters(
    DateTime? DateDiffDayFilter = null,
    DateTime? DateDiffMonthFilter = null,
    DateTime? DateDiffYearFilter = null,
    DateTime? DateDiffHourFilter = null,
    DateTime? DateDiffMinuteFilter = null,
    DateTime? DateDiffSecondFilter = null,
    DateTime? DateDiffMicrosecondFilter = null
);