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
    private const string DateDiffDayMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffDay);
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
    /// Creates DateDiffDay equality over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression"></param>
    /// <param name="filterFunc"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TFilter"></typeparam>
    /// <returns></returns>
    public static IFilterBuilder<TEntity, TFilter> DateDiffDayEqual<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc)
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
                DateDiffDayMethod,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!,
                constantExpression);
            var equalExpression = Equal(callExpression, Constant(0));

            return Lambda<Func<TEntity, bool>>(equalExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
}