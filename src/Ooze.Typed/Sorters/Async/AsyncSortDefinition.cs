using System.Linq.Expressions;

namespace Ooze.Typed.Sorters.Async;

public class AsyncSortDefinition<TEntity, TSorters>
{
    public LambdaExpression DataExpression { get; internal set; } = null!;
    public Func<TSorters, Task<bool>> ShouldRun { get; set; } = null!;
    public Func<TSorters, Task<SortDirection?>> GetSortDirection { get; set; } = null!;
}