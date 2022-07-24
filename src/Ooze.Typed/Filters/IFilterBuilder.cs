using System.Linq.Expressions;

namespace Ooze.Typed.Filters;

public interface IFilterBuilder<TEntity, TFilter>
{
    IFilterBuilder<TEntity, TFilter> Equal<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc);
    IFilterBuilder<TEntity, TFilter> NotEqual<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc);
    IFilterBuilder<TEntity, TFilter> GreaterThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc);
    IFilterBuilder<TEntity, TFilter> LessThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc);
    IFilterBuilder<TEntity, TFilter> In<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, IEnumerable<TProperty>> filterFunc);
    IFilterBuilder<TEntity, TFilter> Range<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, RangeFilter<TProperty>> filterFunc);
    IFilterBuilder<TEntity, TFilter> Custom(
        Func<TFilter, bool> shouldRun,
        Func<TFilter, Expression<Func<TEntity, bool>>> filterExpressionFactory);
    IEnumerable<IFilterDefinition<TEntity, TFilter>> Build();
}

public class RangeFilter<TType>
{
    public TType From { get; set; }
    public TType To { get; set; }
}
