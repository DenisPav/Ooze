namespace Ooze.Typed.Sorters.Async;

internal interface IAsyncSorterHandler<TEntity, in TSorter>
{
    ValueTask<IQueryable<TEntity>> ApplyAsync(
        IQueryable<TEntity> query,
        IEnumerable<TSorter> sorters);
}