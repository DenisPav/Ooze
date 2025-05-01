using System.Linq.Expressions;

namespace Ooze.Typed.Query.Filters;

/// <summary>
/// Interface defining contract for query language filter builder implementation
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IQueryLanguageFilterBuilder<TEntity>
{
    /// <summary>
    /// Fluently creates a custom query language filter definition
    /// </summary>
    /// <param name="dataExpression">Expression specifying property used for filtering</param>
    /// <param name="name">Optional name argument to use in query language queries</param>
    /// <typeparam name="TProperty">Type of targeted property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple query filter definitions</returns>
    IQueryLanguageFilterBuilder<TEntity> Add<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        string? name = null);

    /// <summary>
    /// Return collection of created query language filter definitions
    /// </summary>
    /// <returns>Collection of query language filter definitions</returns>
    IEnumerable<QueryLanguageFilterDefinition<TEntity>> Build();
}