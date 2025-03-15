using System.Linq.Expressions;

namespace Ooze.Typed.Sorters;

/// <summary>
/// Represents a single async sorter definition
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TSorters"></typeparam>
public class SortDefinition<TEntity, TSorters>
{
    /// <summary>
    /// Sorting expression
    /// </summary>
    public LambdaExpression DataExpression { get; internal set; } = null!;

    /// <summary>
    /// Delegate which decides if the sorter should be applied
    /// </summary>
    public Func<TSorters, bool> ShouldRun { get; set; } = null!;

    /// <summary>
    /// Delegate which provides sorting direction
    /// </summary>
    public Func<TSorters, SortDirection?> GetSortDirection { get; set; } = null!;
}