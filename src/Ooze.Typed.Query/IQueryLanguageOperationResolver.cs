using System.Linq.Expressions;
using Ooze.Typed.Paging;

namespace Ooze.Typed.Query;

/// <summary>
/// Ooze resolver instance, contains implementations of Filtering, Query language filtering, Sorting and Paging methods used to update
/// <see cref="IQueryable"/> instances.
/// </summary>
public interface IQueryLanguageOperationResolver
    : IOperationResolver
{
    /// <summary>
    /// Applies valid query language filters over <see cref="IQueryable"/> instance. Filter application is based of query language filter provider
    /// implementation for entity type.
    /// </summary>
    /// <param name="queryable">Base <see cref="IQueryable"/> instance</param>
    /// <param name="query">Query definition</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IQueryable<TEntity> FilterWithQueryLanguage<TEntity>(
        IQueryable<TEntity> queryable,
        string? query);
}

/// <summary>
/// Ooze resolver instance, contains implementations of Filtering, Query language filtering, Sorting and Paging methods used to update
/// <see cref="IQueryable"/> instances. 
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilters">Filter implementation type</typeparam>
/// <typeparam name="TSorters">Sorter implementation type</typeparam>
public interface IQueryLanguageOperationResolver<TEntity, in TFilters, in TSorters>
{
    /// <summary>
    /// Registers passed <see cref="IQueryable"/> instance to <see cref="IQueryLanguageOperationResolver"/> for upcoming operations
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <returns>Resolver fluent instance</returns>
    IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query);

    /// <summary>
    /// Applies valid sorters over <see cref="IQueryable"/> instance. Sorters application is based of sorter provider
    /// implementation for entity type 
    /// </summary>
    /// <param name="sorters">Sorter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <returns>Resolver fluent instance</returns>
    IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> Sort(IEnumerable<TSorters> sorters);

    /// <summary>
    /// Applies valid filters over <see cref="IQueryable"/> instance. Filter application is based of filter provider
    /// implementation for entity type
    /// </summary>
    /// <param name="filters">Filter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <returns>Resolver fluent instance</returns>
    IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> Filter(TFilters filters);

    /// <summary>
    /// Applies valid query language filters over <see cref="IQueryable"/> instance. Filter application is based of query language
    /// filter provider implementation for entity type
    /// </summary>
    /// <param name="query">Query language query to apply over <see cref="IQueryable"/> instance</param>
    /// <returns>Resolver fluent instance</returns>
    IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> FilterWithQueryLanguage(string? query);

    /// <summary>
    /// Applies valid paging options over <see cref="IQueryable"/> instance.
    /// </summary>
    /// <param name="pagingOptions">Instance of paging options</param>
    /// <returns>Resolver fluent instance</returns>
    IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> Page(PagingOptions pagingOptions);

    /// <summary>
    /// Applies valid paging options over <see cref="IQueryable"/> instance. 
    /// </summary>
    /// <param name="cursorPropertyExpression">Expression targeting entity property to use for cursor paging</param>
    /// <param name="pagingOptions">Instance of cursor paging options</param>
    /// <typeparam name="TAfter">After type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Resolver fluent instance</returns>
    IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> PageWithCursor<TAfter, TProperty>(
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions);

    /// <summary>
    /// Return update <see cref="IQueryable"/> instance
    /// </summary>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IQueryable<TEntity> Apply();

    /// <summary>
    /// Applies multiple operations over <see cref="IQueryable"/> instance
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <param name="sorters">Sorter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <param name="filters">Filter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <param name="queryLanguageQuery">Query language query definition</param>
    /// <param name="pagingOptions">Instance of paging options</param>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters,
        TFilters filters,
        string queryLanguageQuery,
        PagingOptions pagingOptions);

    /// <summary>
    /// Applies multiple operations over <see cref="IQueryable"/> instance 
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <param name="sorters">Sorter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <param name="filters">Filter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <param name="queryLanguageQuery">Query language query definition</param>
    /// <param name="cursorPropertyExpression">Expression targeting entity property to use for cursor paging</param>
    /// <param name="pagingOptions">Instance of cursor paging options</param>
    /// <typeparam name="TAfter">After type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IQueryable<TEntity> Apply<TAfter, TProperty>(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters,
        TFilters filters,
        string queryLanguageQuery,
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions);
}