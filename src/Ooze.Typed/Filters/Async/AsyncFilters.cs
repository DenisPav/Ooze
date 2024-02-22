namespace Ooze.Typed.Filters.Async;

/// <summary>
/// Static class with helper methods to start building async filter definitions for specified entity type and filter type
/// </summary>
public static class AsyncFilters
{
    /// <summary>
    /// Creates a new instance of async filter builder for passed entity type and filter type
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Filter builder instance for entity, filter type combination</returns>
    public static IAsyncFilterBuilder<TEntity, TFilter> CreateFor<TEntity, TFilter>()
        => new AsyncFilterBuilder<TEntity, TFilter>();
}