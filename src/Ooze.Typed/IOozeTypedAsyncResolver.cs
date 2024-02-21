using System.Linq.Expressions;
using Ooze.Typed.Paging;

namespace Ooze.Typed;

/// <summary>
/// Ooze resolver instance, contains implementations of Filtering, Sorting and Paging methods used to update
/// <see cref="IQueryable"/> instances.
/// </summary>
public interface IOozeTypedAsyncResolver
{
    /// <summary>
    /// Applies valid sorters over <see cref="IQueryable"/> instance. Sorters application is based of sorter provider
    /// implementation for entity type
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <param name="sorters">Sorter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TSorter">Sorter implementation type</typeparam>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    ValueTask<IQueryable<TEntity>> SortAsync<TEntity, TSorter>(
        IQueryable<TEntity> query,
        IEnumerable<TSorter> sorters);

    /// <summary>
    /// Applies valid filters over <see cref="IQueryable"/> instance. Filter application is based of filter provider
    /// implementation for entity type
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <param name="filters">Filter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilters">Filter implementation type</typeparam>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    ValueTask<IQueryable<TEntity>> FilterAsync<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters filters);

    /// <summary>
    /// Applies valid paging options over <see cref="IQueryable"/> instance.
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <param name="pagingOptions">Instance of paging options</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    ValueTask<IQueryable<TEntity>> PageAsync<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions);

    /// <summary>
    /// Applies valid cursor paging options over <see cref="IQueryable"/> instance. 
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <param name="cursorPropertyExpression">Expression targeting entity property to use for cursor paging</param>
    /// <param name="pagingOptions">Instance of paging options</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TAfter">After type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    ValueTask<IQueryable<TEntity>> PageWithCursorAsync<TEntity, TAfter, TProperty>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions);
}

/// <summary>
/// Ooze resolver instance, contains implementations of Filtering, Sorting and Paging methods used to update
/// <see cref="IQueryable"/> instances. 
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilters">Filter implementation type</typeparam>
/// <typeparam name="TSorters">Sorter implementation type</typeparam>
public interface IOozeTypedAsyncResolver<TEntity, in TFilters, in TSorters>
{
    /// <summary>
    /// Registers passed <see cref="IQueryable"/> instance to <see cref="IOozeTypedResolver"/> for upcoming operations
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <returns>Resolver fluent instance</returns>
    IOozeTypedAsyncResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query);

    /// <summary>
    /// Applies valid sorters over <see cref="IQueryable"/> instance. Sorters application is based of sorter provider
    /// implementation for entity type 
    /// </summary>
    /// <param name="sorters">Sorter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IOozeTypedAsyncResolver<TEntity, TFilters, TSorters> Sort(IEnumerable<TSorters> sorters);

    /// <summary>
    /// Applies valid filters over <see cref="IQueryable"/> instance. Filter application is based of filter provider
    /// implementation for entity type
    /// </summary>
    /// <param name="filters">Filter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IOozeTypedAsyncResolver<TEntity, TFilters, TSorters> Filter(TFilters filters);

    /// <summary>
    /// Applies valid paging options over <see cref="IQueryable"/> instance.
    /// </summary>
    /// <param name="pagingOptions">Instance of paging options</param>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IOozeTypedAsyncResolver<TEntity, TFilters, TSorters> Page(PagingOptions pagingOptions);

    /// <summary>
    /// Apply and return update <see cref="IQueryable"/> instance asynchronously
    /// </summary>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    ValueTask<IQueryable<TEntity>> ApplyAsync();
}