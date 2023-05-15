namespace Ooze.Typed.Filters;

internal interface IOozeFilterHandler<TEntity, in TFilter>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        TFilter filters);
}
