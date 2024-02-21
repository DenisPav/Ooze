using System.Linq.Expressions;

namespace Ooze.Typed.Filters;

public class FilterDefinition<TEntity, TFilter>
{
    public Func<TFilter, bool> ShouldRun { get; internal set; } = null!;
    public Func<TFilter, Expression<Func<TEntity, bool>>> FilterExpressionFactory { get; internal set; } = null!;
}