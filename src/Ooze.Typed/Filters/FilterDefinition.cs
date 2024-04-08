using System.Linq.Expressions;

namespace Ooze.Typed.Filters;

/// <summary>
/// Represents a single filter definition
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilter">Filter type</typeparam>
public class FilterDefinition<TEntity, TFilter>
{
    /// <summary>
    /// Delegate which decides if the filter should be applied
    /// </summary>
    public Func<TFilter, bool> ShouldRun { get; set; } = null!;
    
    /// <summary>
    /// Delegate which creates final expression used for filtering
    /// </summary>
    public Func<TFilter, Expression<Func<TEntity, bool>>> FilterExpressionFactory { get; set; } = null!;
}