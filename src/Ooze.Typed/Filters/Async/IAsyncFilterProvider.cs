namespace Ooze.Typed.Filters.Async;

/// <summary>
/// Async filter provider interface called internally by <see cref="IAsyncOperationResolver"/>/
/// <see cref="IAsyncOperationResolver{TEntity,TFilters,TSorters}"/> to fetch defined filters for provided Entity type.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilter">Filter implementation type</typeparam>
public interface IAsyncFilterProvider<TEntity, TFilter>
{
    /// <summary>
    /// Method used for creation of <see cref="AsyncFilterDefinition{TEntity,TFilter}"/> definitions. These definitions are
    /// used in filtering process.
    /// </summary>
    /// <returns>Collection of filter definitions</returns>
    ValueTask<IEnumerable<AsyncFilterDefinition<TEntity, TFilter>>> GetFiltersAsync();
}