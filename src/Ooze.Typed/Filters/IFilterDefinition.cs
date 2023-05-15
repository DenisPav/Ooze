namespace Ooze.Typed.Filters;

/// <summary>
/// Marker interface used for holding information about filter definition for specified entity, filter type combination 
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilter">Filter type</typeparam>
public interface IFilterDefinition<TEntity, TFilter>
{
}