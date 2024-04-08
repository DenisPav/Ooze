namespace Ooze.Typed.Sorters.Async;

/// <summary>
/// Async sorter provider interface called internally by <see cref="IOperationResolver"/>/
/// <see cref="IOperationResolver{TEntity,TFilters,TSorters}"/> to fetch defined sorters for provided Entity type.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TSorters">Sorter implementation type</typeparam>
public interface IAsyncSorterProvider<TEntity, TSorters>
{
    /// <summary>
    /// Method used for creation of <see cref="AsyncSortDefinition{TEntity,TSorters}"/> definitions. These definitions are
    /// used in sorting process.
    /// </summary>
    /// <returns>Collection of sort definitions</returns>
    ValueTask<IEnumerable<AsyncSortDefinition<TEntity, TSorters>>> GetSortersAsync();
}