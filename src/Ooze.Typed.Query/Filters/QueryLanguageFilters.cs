namespace Ooze.Typed.Query.Filters;

public static class QueryLanguageFilters
{
    public static IQueryLanguageFilterBuilder<TEntity> CreateFor<TEntity>()
        => new QueryLanguageFilterBuilder<TEntity>();
}