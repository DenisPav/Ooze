using System.Linq.Expressions;

namespace Ooze.Typed.Filters;

/// <summary>
/// Interface defining contract for filter builder implementation
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilter">Filter type</typeparam>
public interface IFilterBuilder<TEntity, TFilter>
{
    /// <summary>
    /// Fluently creates "Equality" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> Equal<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc);

    /// <summary>
    /// Fluently creates "Not Equal" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> NotEqual<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc);

    /// <summary>
    /// Fluently creates "Greater Than" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> GreaterThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc);

    /// <summary>
    /// Fluently creates "Less Than" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> LessThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc);

    /// <summary>
    /// Fluently creates "In" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> In<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, IEnumerable<TProperty>> filterFunc);

    /// <summary>
    /// Fluently creates "Not In" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> NotIn<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, IEnumerable<TProperty>> filterFunc);

    /// <summary>
    /// Fluently creates "Range" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> Range<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, RangeFilter<TProperty>> filterFunc);

    /// <summary>
    /// Fluently creates "Out of Range" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> OutOfRange<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, RangeFilter<TProperty>> filterFunc);

    /// <summary>
    /// Fluently creates "Starts with" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> StartsWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc);

    /// <summary>
    /// Fluently creates "Doesn't Start With" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> DoesntStartWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc);

    /// <summary>
    /// Fluently creates "Ends With" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> EndsWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc);

    /// <summary>
    /// Fluently creates "Doesn't End With" filter definition
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="filterFunc">Delegate targeting property of filter class</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> DoesntEndWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc);

    /// <summary>
    /// Fluently creates custom filter definition
    /// </summary>
    /// <param name="shouldRun">Delegate showing if the filter should execute</param>
    /// <param name="filterExpressionFactory">Factory for creation of filter Expression</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IFilterBuilder<TEntity, TFilter> Add(
        Func<TFilter, bool> shouldRun,
        Func<TFilter, Expression<Func<TEntity, bool>>> filterExpressionFactory);

    /// <summary>
    /// Return collection of created filter definitions
    /// </summary>
    /// <returns>Collection of filter definitions</returns>
    IEnumerable<IFilterDefinition<TEntity, TFilter>> Build();
}
