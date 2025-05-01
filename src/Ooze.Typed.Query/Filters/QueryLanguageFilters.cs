namespace Ooze.Typed.Query.Filters;

/// <summary>
/// Static class with helper methods to start building query language filter definitions for specified entity type
/// </summary>
public static class QueryLanguageFilters
{
    /// <summary>
    /// Creates a new instance of query language filter builder for passed entity type
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Query language filter builder instance for entity</returns>
    public static IQueryLanguageFilterBuilder<TEntity> CreateFor<TEntity>()
        => new QueryLanguageFilterBuilder<TEntity>();
}