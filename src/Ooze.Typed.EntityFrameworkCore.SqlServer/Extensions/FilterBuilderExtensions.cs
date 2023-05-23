using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Expressions;
using Ooze.Typed.Filters;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.SqlServer.Extensions;

/// <summary>
/// Sqlite extensions for FilterBuilder
/// </summary>
public static class FilterBuilderExtensions
{
    private static readonly Type DbFunctionsExtensionsType = typeof(SqlServerDbFunctionsExtensions);
    private const string IsDateMethod = nameof(SqlServerDbFunctionsExtensions.IsDate);
    private const string IsNumericMethod = nameof(SqlServerDbFunctionsExtensions.IsNumeric);
    private const string ContainsMethod = nameof(SqlServerDbFunctionsExtensions.Contains);
    private const string DateDiffDayMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffDay);
    private const string DateDiffMonthMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffMonth);
    private const string DateDiffWeekMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffWeek);
    private const string DateDiffYearMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffYear);
    private const string DateDiffHourMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffHour);
    private const string DateDiffMinuteMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffMinute);
    private const string DateDiffSecondMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffSecond);
    private const string DateDiffMillisecondMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffMillisecond);
    private const string DateDiffMicrosecondMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffMicrosecond);
    private const string DateDiffNanosecondMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffNanosecond);
    private static readonly MemberExpression EfPropertyExpression = Property(null, typeof(EF), nameof(EF.Functions));

    /// <summary>
    /// Creates a IsDate filter over a string property if filter requests it
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDate<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, bool?> filterFunc)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter).GetValueOrDefault() == true;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter).GetValueOrDefault();
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var callExpression = Call(
                DbFunctionsExtensionsType,
                IsDateMethod,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!);
            Expression notExpression = filterValue == true
                ? callExpression
                : Not(callExpression);

            return Lambda<Func<TEntity, bool>>(notExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }

    /// <summary>
    /// Creates a IsNumeric filter over a string property if filter requests it
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsNumeric<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, bool?> filterFunc)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter).GetValueOrDefault() == true;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter).GetValueOrDefault();
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var callExpression = Call(
                DbFunctionsExtensionsType,
                IsNumericMethod,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!);
            Expression notExpression = filterValue == true
                ? callExpression
                : Not(callExpression);

            return Lambda<Func<TEntity, bool>>(notExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
    
    /// <summary>
    /// Creates a Contains filter over specified property and filter
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <typeparam name="TProperty">Targeted property type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> Contains<TEntity, TFilter, TProperty>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, TProperty?>> dataExpression,
        Func<TFilter, string?> filterFunc)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter) != null;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter);
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var constantExpression = BasicExpressions.GetWrappedConstantExpression(filterValue);
            var callExpression = Call(
                DbFunctionsExtensionsType,
                ContainsMethod,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!,
                constantExpression);
            
            return Lambda<Func<TEntity, bool>>(callExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }

    /// <summary>
    /// Creates DateDiffDay filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffDay<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffDayMethod, dataExpression, filterFunc, operation, diffConstant);

    /// <summary>
    /// Creates DateDiffMonth filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffMonth<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffMonthMethod, dataExpression, filterFunc, operation, diffConstant);

    /// <summary>
    /// Creates DateDiffWeek filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffWeek<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffWeekMethod, dataExpression, filterFunc, operation, diffConstant);

    /// <summary>
    /// Creates DateDiffYear filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffYear<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffYearMethod, dataExpression, filterFunc, operation, diffConstant);

    /// <summary>
    /// Creates DateDiffHour filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffHour<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffHourMethod, dataExpression, filterFunc, operation, diffConstant);

    /// <summary>
    /// Creates DateDiffMinute filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffMinute<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffMinuteMethod, dataExpression, filterFunc, operation, diffConstant);

    /// <summary>
    /// Creates DateDiffSecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffSecond<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffSecondMethod, dataExpression, filterFunc, operation, diffConstant);

    /// <summary>
    /// Creates DateDiffMillisecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffMillisecond<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffMillisecondMethod, dataExpression, filterFunc, operation,
            diffConstant);

    /// <summary>
    /// Creates DateDiffMicrosecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffMicrosecond<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffMicrosecondMethod, dataExpression, filterFunc, operation,
            diffConstant);
    
    /// <summary>
    /// Creates DateDiffNanosecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffNanosecond<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffNanosecondMethod, dataExpression, filterFunc, operation,
            diffConstant);

    private static IFilterBuilder<TEntity, TFilter> IsDateDiffFilter<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        string methodName,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter) != null;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter).GetValueOrDefault();
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var constantExpression = BasicExpressions.GetWrappedConstantExpression(filterValue);
            var callExpression = Call(
                DbFunctionsExtensionsType,
                methodName,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!,
                constantExpression);

            var operationExpressionFactory = GetOperationFactory(operation);
            var operationExpression = operationExpressionFactory(callExpression, Constant(diffConstant));
            return Lambda<Func<TEntity, bool>>(operationExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }

    private static Func<Expression, Expression, Expression> GetOperationFactory(DateDiffOperation operation)
        => operation switch
        {
            DateDiffOperation.GreaterThan => GreaterThan,
            DateDiffOperation.LessThan => LessThan,
            DateDiffOperation.Equal => Equal,
            DateDiffOperation.NotEqual => (callExpr, constant) => Not(Equal(callExpr, constant)),
            DateDiffOperation.NotGreaterThan => (callExpr, constant) => Not(GreaterThan(callExpr, constant)),
            DateDiffOperation.NotLessThan => (callExpr, constant) => Not(LessThan(callExpr, constant)),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
}