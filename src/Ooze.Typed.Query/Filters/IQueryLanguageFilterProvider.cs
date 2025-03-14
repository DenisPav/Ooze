namespace Ooze.Typed.Query.Filters;

public interface IQueryLanguageFilterProvider<TEntity>
{
    IEnumerable<QueryLanguageFilterDefinition<TEntity>> GetMappings();
}