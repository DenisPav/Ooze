using System.Linq.Expressions;

namespace Ooze.Typed.Sorters.Async;

/// <summary>
/// Represents a single async sorter definition
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TSorters"></typeparam>
public class AsyncSortDefinition<TEntity, TSorters>
{
    /// <summary>
    /// Sorting expression
    /// </summary>
    public LambdaExpression DataExpression { get; init; } = null!;
    
    /// <summary>
    /// Delegate which decides if the sorter should be applied
    /// </summary>
    public Func<TSorters, Task<bool>> ShouldRun { get; init; } = null!;
    
    /// <summary>
    /// Delegate which provides sorting direction
    /// </summary>
    public Func<TSorters, Task<SortDirection?>> GetSortDirection { get; init; } = null!;
}