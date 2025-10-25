using System.Linq.Expressions;

namespace Ooze.Typed.Filters;

/// <summary>
/// Interface with shared filter contracts
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilter">Filter type</typeparam>
/// <typeparam name="TReturn">Return type</typeparam>
public interface IFilters<TEntity, out TFilter, out TReturn>
{
    /// <summary>
    /// Fluently creates "Equality" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn Equal<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Not Equal" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn NotEqual<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Greater Than" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn GreaterThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Less Than" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn LessThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "In" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn In<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, IEnumerable<TProperty>?> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Not In" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn NotIn<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, IEnumerable<TProperty>?> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Range" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn Range<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, RangeFilter<TProperty>?> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Out of Range" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn OutOfRange<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, RangeFilter<TProperty>?> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Starts with" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn StartsWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Doesn't Start With" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn DoesntStartWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Ends With" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn EndsWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates "Doesn't End With" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if filter should be applied</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn DoesntEndWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc,
        Func<TFilter, bool>? shouldRun = null);

    /// <summary>
    /// Fluently creates custom filter definition
    /// </summary>
    /// <param name="shouldRun">Delegate showing if the filter should execute</param>
    /// <param name="filterExpressionFactory">Factory for creation of filter Expression</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    TReturn Add(
        Func<TFilter, bool> shouldRun,
        Func<TFilter, Expression<Func<TEntity, bool>>> filterExpressionFactory);
}