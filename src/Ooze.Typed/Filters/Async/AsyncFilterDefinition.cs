using System.Linq.Expressions;

namespace Ooze.Typed.Filters.Async;

public class AsyncFilterDefinition<TEntity, TFilter>
{
    public Func<TFilter, Task<bool>> ShouldRun { get; internal set; } = null!;
    public Func<TFilter, Task<Expression<Func<TEntity, bool>>>> FilterExpressionFactory { get; internal set; } = null!;
}