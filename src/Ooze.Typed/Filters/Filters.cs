namespace Ooze.Typed.Filters;

/// <summary>
/// Static class with helper methods to start building filter definitions for specified entity type and filter type
/// </summary>
public static class Filters
{
    /// <summary>
    /// Creates a new instance of filter builder for passed entity type and filter type
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Filter builder instance for entity, filter type combination</returns>
    public static IFilterBuilder<TEntity, TFilter> CreateFor<TEntity, TFilter>()
        => new FilterBuilder<TEntity, TFilter>();
}
