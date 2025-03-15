using System.Reflection;

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
    /// Targeted property information for single filter definition 
    /// </summary>
    public PropertyInfo TargetProperty { get; init; } = null!;
}