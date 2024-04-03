using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.MySql.Extensions;

internal static class Shared
{
    public static readonly Type DbFunctionsExtensionsType = typeof(MySqlDbFunctionsExtensions);
    public const string DateDiffDayMethod = nameof(MySqlDbFunctionsExtensions.DateDiffDay);
    public const string DateDiffMonthMethod = nameof(MySqlDbFunctionsExtensions.DateDiffMonth);
    public const string DateDiffYearMethod = nameof(MySqlDbFunctionsExtensions.DateDiffYear);
    public const string DateDiffHourMethod = nameof(MySqlDbFunctionsExtensions.DateDiffHour);
    public const string DateDiffMinuteMethod = nameof(MySqlDbFunctionsExtensions.DateDiffMinute);
    public const string DateDiffSecondMethod = nameof(MySqlDbFunctionsExtensions.DateDiffSecond);
    public const string DateDiffMicrosecondMethod = nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond);
    public static readonly MemberExpression EfPropertyExpression = Property(null, typeof(EF), nameof(EF.Functions));
}