namespace Ooze.Typed.Filters;

public interface IOozeFilterProvider<TEntity, TFilter>
{
    IEnumerable<IFilterDefinition<TEntity, TFilter>> GetFilters();
}
