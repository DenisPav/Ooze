namespace Ooze.Typed.Sorters;

internal interface ISorterHandler<TEntity, in TSorter>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorter> sorters);
}