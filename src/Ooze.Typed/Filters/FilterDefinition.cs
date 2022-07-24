using System.Linq.Expressions;

namespace Ooze.Typed.Filters;

internal class FilterDefinition<TEntity, TFilter> : IFilterDefinition<TEntity, TFilter>
{
    public LambdaExpression DataExpression { get; internal set; }
    public Func<TFilter, bool> ShouldRun { get; internal set; }
    public Func<TFilter, Expression<Func<TEntity, bool>>> FilterExpressionFactory { get; internal set; }
}