namespace Ooze.Typed.Query.Filters;

public interface IQueryFilterProvider<TEntity>
{
    IEnumerable<QueryFilterDefinition<TEntity>> GetMappings();
}