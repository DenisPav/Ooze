using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Expressions;
using Ooze.Typed.Filters;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.MySql.Extensions;

/// <summary>
/// MySql/MariaDb extensions for FilterBuilder
/// </summary>
public static class FilterBuilderExtensions
{
    private static readonly Type DbFunctionsExtensionsType = typeof(MySqlDbFunctionsExtensions);
    private const string MatchMethod = nameof(MySqlDbFunctionsExtensions.Match);
    private static readonly MemberExpression EfPropertyExpression = Property(null, typeof(EF), nameof(EF.Functions));
    
    /// <summary>
    /// Creates a Match filter over specified property and filter
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <typeparam name="TProperty">Targeted property type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsMatch<TEntity, TFilter, TProperty>(
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
                MatchMethod,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!,
                constantExpression);

            return Lambda<Func<TEntity, bool>>(callExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
}