namespace Ooze.Typed.Sorters;

/// <summary>
/// Static class with helper methods to start building sorter definitions for specified entity type and sorters type
/// </summary>
public static class Sorters
{
    /// <summary>
    /// Creates a new instance of sorter builder for passed entity type and sorter type
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TSorters">Sorter type</typeparam>
    /// <returns>Sorter builder instance for entity, sorter type combination</returns>
    public static ISorterBuilder<TEntity, TSorters> CreateFor<TEntity, TSorters>()
        => new SorterBuilder<TEntity, TSorters>();
}
