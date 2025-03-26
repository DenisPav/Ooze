using System.Linq.Expressions;

namespace Ooze.Typed.Query.Filters;

/// <summary>
/// Represents a single query language filter definition
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public class QueryLanguageFilterDefinition<TEntity>
{
    /// <summary>
    /// Optional name arguments used in query language queries
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// Targeted member expression used for a single filter definition
    /// </summary>
    public MemberExpression MemberExpression { get; init; } = null!;
}