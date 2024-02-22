namespace Ooze.Typed.Sorters.Async;

/// <summary>
/// Static class with helper methods to start building async sorter definitions for specified entity type and sorters type
/// </summary>
public static class AsyncSorters
{
    /// <summary>
    /// Creates a new instance of async sorter builder for passed entity type and sorter type
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TSorters">Sorter type</typeparam>
    /// <returns>Sorter builder instance for entity, sorter type combination</returns>
    public static IAsyncSorterBuilder<TEntity, TSorters> CreateFor<TEntity, TSorters>()
        => new AsyncSorterBuilder<TEntity, TSorters>();
}