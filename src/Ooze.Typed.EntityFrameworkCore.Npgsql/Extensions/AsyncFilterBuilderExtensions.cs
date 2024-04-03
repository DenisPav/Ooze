using System.Linq.Expressions;
using Ooze.Typed.Expressions;
using Ooze.Typed.Filters.Async;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.Npgsql.Extensions;

/// <summary>
/// Postgres extensions for AsyncFilterBuilder
/// </summary>
public static class AsyncFilterBuilderExtensions
{
    /// <summary>
    /// Creates a ILike filter over specified property and filter
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <typeparam name="TProperty">Targeted property type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> InsensitiveLike<TEntity, TFilter, TProperty>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
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
            var callExpression = Call(Shared.DbFunctionsExtensionsType, Shared.ILikeMethod,
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
    /// Creates a Soundex filter over specified property and filter
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IAsyncFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> SoundexEqual<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string?>> dataExpression,
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
            var callExpression = Call(Shared.FuzzyStringMatchDbFunctionsExtensionsType, Shared.FuzzyStringMatchSoundexMethod,
                Type.EmptyTypes, Shared.EfPropertyExpression,
                memberAccessExpression);
            var equalExpression = Equal(callExpression, constantExpression);
            return Lambda<Func<TEntity, bool>>(equalExpression, parameterExpression);
        }

        shouldRun ??= FilterShouldRun;
        filterBuilder.Add(shouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
}
