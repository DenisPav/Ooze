using System.Linq.Expressions;

namespace Ooze.Typed.Filters;

internal class FilterDefinition<TEntity, TFilter> : IFilterDefinition<TEntity, TFilter>
{
    public LambdaExpression DataExpression { get; internal init; } = null!;
    public Func<TFilter, bool> ShouldRun { get; internal set; } = null!;
    public Func<TFilter, Expression<Func<TEntity, bool>>> FilterExpressionFactory { get; internal set; } = null!;
}