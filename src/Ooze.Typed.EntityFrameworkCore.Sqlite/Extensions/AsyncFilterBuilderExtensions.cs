using System.Linq.Expressions;
using Ooze.Typed.Expressions;
using Ooze.Typed.Filters.Async;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.Sqlite.Extensions;

/// <summary>
/// Sqlite extensions for AsyncFilterBuilder
/// </summary>
public static class AsyncFilterBuilderExtensions
{
    /// <summary>
    /// Applies a Glob filter over specified entity property and passed filter glob expression
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property for glob filtering</param>
    /// <param name="filterFunc">Filter delegate targeting property with glob expression</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> Glob<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string?>> dataExpression,
        Func<TFilter, string?> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        bool FilterShouldRun(TFilter filter) => string.IsNullOrEmpty(filterFunc(filter)) == false;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter);
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var constantExpression = BasicExpressions.GetWrappedConstantExpression(filterValue);
            var callExpression = Call(Shared.DbFunctionsExtensionsType, Shared.GlobMethod,
                Type.EmptyTypes, Shared.EfPropertyExpression,
                memberAccessExpression!,
                constantExpression);

            return Lambda<Func<TEntity, bool>>(callExpression, parameterExpression);
        }

        shouldRun ??= FilterShouldRun;
        filterBuilder.Add(shouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
}