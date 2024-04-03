using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.EntityFrameworkCore.SqlServer.Extensions;

internal static class Shared
{
    public const string IsDateMethod = nameof(SqlServerDbFunctionsExtensions.IsDate);
    public const string IsNumericMethod = nameof(SqlServerDbFunctionsExtensions.IsNumeric);
    public const string ContainsMethod = nameof(SqlServerDbFunctionsExtensions.Contains);
    public const string DateDiffDayMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffDay);
    public const string DateDiffMonthMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffMonth);
    public const string DateDiffWeekMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffWeek);
    public const string DateDiffYearMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffYear);
    public const string DateDiffHourMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffHour);
    public const string DateDiffMinuteMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffMinute);
    public const string DateDiffSecondMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffSecond);
    public const string DateDiffMillisecondMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffMillisecond);
    public const string DateDiffMicrosecondMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffMicrosecond);
    public const string DateDiffNanosecondMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffNanosecond);
    public static readonly Type DbFunctionsExtensionsType = typeof(SqlServerDbFunctionsExtensions);
    public static readonly MemberExpression EfPropertyExpression = Expression.Property(null, typeof(EF), nameof(EF.Functions));
}