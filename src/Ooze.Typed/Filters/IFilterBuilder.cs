namespace Ooze.Typed.Filters;

/// <summary>
/// Interface defining contract for filter builder implementation
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilter">Filter type</typeparam>
public interface IFilterBuilder<TEntity, TFilter> : IFilters<TEntity, TFilter, IFilterBuilder<TEntity, TFilter>>
{
    /// <summary>
    /// Return collection of created filter definitions
    /// </summary>
    /// <returns>Collection of filter definitions</returns>
    IEnumerable<FilterDefinition<TEntity, TFilter>> Build();
}