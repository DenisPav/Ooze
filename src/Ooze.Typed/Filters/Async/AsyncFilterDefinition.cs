using System.Linq.Expressions;

namespace Ooze.Typed.Filters.Async;

/// <summary>
/// Represents a single async filter definition
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TFilter">Filter type</typeparam>
public class AsyncFilterDefinition<TEntity, TFilter>
{
    /// <summary>
    /// Delegate which decides if the filter should be applied
    /// </summary>
    public Func<TFilter, Task<bool>> ShouldRun { get; init; } = null!;

    /// <summary>
    /// Delegate which creates final expression used for filtering
    /// </summary>
    public Func<TFilter, Task<Expression<Func<TEntity, bool>>>> FilterExpressionFactory { get; init; } = null!;
}