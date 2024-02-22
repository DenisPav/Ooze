namespace Ooze.Typed.Filters;

internal interface IFilterHandler<TEntity, in TFilter>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        TFilter filters);
}