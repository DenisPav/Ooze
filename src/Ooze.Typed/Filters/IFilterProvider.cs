namespace Ooze.Typed.Filters;

/// <summary>
/// Filter provider interface called internally by <see cref="IOperationResolver"/>/
/// <see cref="IOperationResolver{TEntity,TFilters,TSorters}"/> to fetch defined filters for provided Entity type.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilter">Filter implementation type</typeparam>
public interface IFilterProvider<TEntity, TFilter>
{
    /// <summary>
    /// Method used for creation of <see cref="FilterDefinition{TEntity,TFilter}"/> definitions. These definitions are
    /// used in filtering process.
    /// </summary>
    /// <returns>Collection of filter definitions</returns>
    IEnumerable<FilterDefinition<TEntity, TFilter>> GetFilters();
}