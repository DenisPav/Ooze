namespace Ooze.Typed.Query.Filters;

/// <summary>
/// Query language filter provider interface called internally by <see cref="IQueryLanguageOperationResolver"/>/
/// <see cref="IQueryLanguageOperationResolver{TEntity,TFilters,TSorters}"/> to fetch defined filters for provided Entity type.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IQueryLanguageFilterProvider<TEntity>
{
    /// <summary>
    /// Method used for creation of <see cref="QueryLanguageFilterDefinition{TEntity}"/> definitions. These definitions are
    /// used in filtering process.
    /// </summary>
    /// <returns>Collection of query language filter definitions</returns>
    IEnumerable<QueryLanguageFilterDefinition<TEntity>> GetMappings();
}