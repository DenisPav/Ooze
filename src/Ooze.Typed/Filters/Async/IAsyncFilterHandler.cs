namespace Ooze.Typed.Filters.Async;

internal interface IAsyncFilterHandler<TEntity, in TFilter>
{
    ValueTask<IQueryable<TEntity>> ApplyAsync(
        IQueryable<TEntity> query,
        TFilter filters);
}