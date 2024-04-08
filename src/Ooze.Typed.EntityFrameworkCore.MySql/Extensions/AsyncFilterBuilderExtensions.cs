using System.Linq.Expressions;
using Ooze.Typed.Expressions;
using Ooze.Typed.Filters.Async;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.MySql.Extensions;

/// <summary>
/// MySql/MariaDb extensions for AsyncFilterBuilder
/// </summary>
public static class AsyncFilterBuilderExtensions
{
    /// <summary>
    /// Creates DateDiffDay filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> IsDateDiffDay<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffDayMethod, dataExpression, filterFunc, operation, diffConstant);
    
    /// <summary>
    /// Creates DateDiffMonth filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> IsDateDiffMonth<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(
            Shared.DateDiffMonthMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);
    
    /// <summary>
    /// Creates DateDiffYear filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> IsDateDiffYear<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(
            Shared.DateDiffYearMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);
    
    /// <summary>
    /// Creates DateDiffHour filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> IsDateDiffHour<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(
            Shared.DateDiffHourMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);
    
    /// <summary>
    /// Creates DateDiffMinute filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> IsDateDiffMinute<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(
            Shared.DateDiffMinuteMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);
    
    /// <summary>
    /// Creates DateDiffSecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> IsDateDiffSecond<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(
            Shared.DateDiffSecondMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);
    
    /// <summary>
    /// Creates DateDiffMicrosecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> IsDateDiffMicrosecond<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(
            Shared.DateDiffMicrosecondMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);
    
    private static IAsyncFilterBuilder<TEntity, TFilter> IsDateDiffFilter<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        string methodName,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter) != null;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter).GetValueOrDefault();
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var constantExpression = BasicExpressions.GetWrappedConstantExpression(filterValue);
            var callExpression = Call(
                Shared.DbFunctionsExtensionsType,
                methodName,
                Type.EmptyTypes,
                Shared.EfPropertyExpression,
                memberAccessExpression!,
                constantExpression);

            var operationExpressionFactory = GetOperationFactory(operation);
            var operationExpression = operationExpressionFactory(callExpression, Constant(diffConstant));
            return Lambda<Func<TEntity, bool>>(operationExpression, parameterExpression);
        }

        shouldRun ??= FilterShouldRun;
        filterBuilder.Add(shouldRun, FilterExpressionFactory);
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