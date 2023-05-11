namespace Ooze.Typed.Sorters;

internal interface IOozeSorterHandler<TEntity, in TSorter>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorter> sorters);
}
