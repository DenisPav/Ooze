﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Expressions;
using Ooze.Typed.Filters;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.SqlServer.Extensions;

/// <summary>
/// Sql Server extensions for FilterBuilder
/// </summary>
public static class FilterBuilderExtensions
{
    /// <summary>
    /// Creates a IsDate filter over a string property if filter requests it
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDate<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string?>> dataExpression,
        Func<TFilter, bool?> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter).GetValueOrDefault() == true;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter).GetValueOrDefault();
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var callExpression = Call(Shared.DbFunctionsExtensionsType, Shared.IsDateMethod,
                Type.EmptyTypes, Shared.EfPropertyExpression,
                memberAccessExpression);
            Expression notExpression = filterValue == true
                ? callExpression
                : Not(callExpression);

            return Lambda<Func<TEntity, bool>>(notExpression, parameterExpression);
        }

        shouldRun ??= FilterShouldRun;
        filterBuilder.Add(shouldRun, FilterExpressionFactory);
        return filterBuilder;
    }

    /// <summary>
    /// Creates a IsNumeric filter over a string property if filter requests it
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsNumeric<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string?>> dataExpression,
        Func<TFilter, bool?> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter).GetValueOrDefault() == true;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter).GetValueOrDefault();
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var callExpression = Call(Shared.DbFunctionsExtensionsType, Shared.IsNumericMethod,
                Type.EmptyTypes, Shared.EfPropertyExpression,
                memberAccessExpression);
            Expression notExpression = filterValue == true
                ? callExpression
                : Not(callExpression);

            return Lambda<Func<TEntity, bool>>(notExpression, parameterExpression);
        }

        shouldRun ??= FilterShouldRun;
        filterBuilder.Add(shouldRun, FilterExpressionFactory);
        return filterBuilder;
    }

    /// <summary>
    /// Creates a Contains filter over specified property and filter
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <typeparam name="TProperty">Targeted property type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> Contains<TEntity, TFilter, TProperty>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, TProperty?>> dataExpression,
        Func<TFilter, string?> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter) != null;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter);
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var constantExpression = BasicExpressions.GetWrappedConstantExpression(filterValue);
            var callExpression = Call(Shared.DbFunctionsExtensionsType, Shared.ContainsMethod,
                Type.EmptyTypes, Shared.EfPropertyExpression,
                memberAccessExpression,
                constantExpression);

            return Lambda<Func<TEntity, bool>>(callExpression, parameterExpression);
        }

        shouldRun ??= FilterShouldRun;
        filterBuilder.Add(shouldRun, FilterExpressionFactory);
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
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffDay<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffDayMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    /// <summary>
    /// Creates DateDiffMonth filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffMonth<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffMonthMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    /// <summary>
    /// Creates DateDiffWeek filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffWeek<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffWeekMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    /// <summary>
    /// Creates DateDiffYear filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffYear<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffYearMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    /// <summary>
    /// Creates DateDiffHour filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffHour<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffHourMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    /// <summary>
    /// Creates DateDiffMinute filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffMinute<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffMinuteMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    /// <summary>
    /// Creates DateDiffSecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffSecond<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffSecondMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    /// <summary>
    /// Creates DateDiffMillisecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffMillisecond<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffMillisecondMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    /// <summary>
    /// Creates DateDiffMicrosecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffMicrosecond<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffMicrosecondMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    /// <summary>
    /// Creates DateDiffNanosecond filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffNanosecond<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0,
        Func<TFilter, bool>? shouldRun = null)
        => filterBuilder.IsDateDiffFilter(Shared.DateDiffNanosecondMethod,
            dataExpression,
            filterFunc,
            operation,
            diffConstant,
            shouldRun);

    private static IFilterBuilder<TEntity, TFilter> IsDateDiffFilter<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
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
            var callExpression = Call(Shared.DbFunctionsExtensionsType,
                methodName,
                Type.EmptyTypes, Shared.EfPropertyExpression,
                memberAccessExpression,
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