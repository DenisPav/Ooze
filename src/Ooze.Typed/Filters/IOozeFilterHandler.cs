namespace Ooze.Typed.Filters;

public interface IOozeFilterHandler<TEntity, TFilter>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        TFilter filters);
}
