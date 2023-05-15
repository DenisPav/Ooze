using Ooze.Typed.Paging;

namespace Ooze.Typed;

/// <summary>
/// Ooze resolver instance, contains implementations of Filtering, Sorting and Paging methods used to update
/// <see cref="IQueryable"/> instances.
/// </summary>
public interface IOozeTypedResolver
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
    IQueryable<TEntity> Sort<TEntity, TSorter>(
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
    IQueryable<TEntity> Filter<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters filters);

    /// <summary>
    /// Applies valid paging options over <see cref="IQueryable"/> instance.
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <param name="pagingOptions">Instance of paging options</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IQueryable<TEntity> Page<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions);
}

/// <summary>
/// Ooze resolver instance, contains implementations of Filtering, Sorting and Paging methods used to update
/// <see cref="IQueryable"/> instances. 
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilters">Filter implementation type</typeparam>
/// <typeparam name="TSorters">Sorter implementation type</typeparam>
public interface IOozeTypedResolver<TEntity, in TFilters, in TSorters>
{
    /// <summary>
    /// Registers passed <see cref="IQueryable"/> instance to <see cref="IOozeTypedResolver"/> for upcoming operations
    /// </summary>
    /// <param name="query">Base <see cref="IQueryable"/> instance</param>
    /// <returns>Resolver fluent instance</returns>
    IOozeTypedResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query);

    /// <summary>
    /// Applies valid sorters over <see cref="IQueryable"/> instance. Sorters application is based of sorter provider
    /// implementation for entity type 
    /// </summary>
    /// <param name="sorters">Sorter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IOozeTypedResolver<TEntity, TFilters, TSorters> Sort(IEnumerable<TSorters> sorters);

    /// <summary>
    /// Applies valid filters over <see cref="IQueryable"/> instance. Filter application is based of filter provider
    /// implementation for entity type
    /// </summary>
    /// <param name="filters">Filter definitions to apply over <see cref="IQueryable"/> instance</param>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IOozeTypedResolver<TEntity, TFilters, TSorters> Filter(TFilters filters);

    /// <summary>
    /// Applies valid paging options over <see cref="IQueryable"/> instance.
    /// </summary>
    /// <param name="pagingOptions">Instance of paging options</param>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IOozeTypedResolver<TEntity, TFilters, TSorters> Page(PagingOptions pagingOptions);

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
    /// <param name="pagingOptions">Instance of paging options</param>
    /// <returns>Updated <see cref="IQueryable"/> instance</returns>
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters,
        TFilters filters,
        PagingOptions pagingOptions);
}