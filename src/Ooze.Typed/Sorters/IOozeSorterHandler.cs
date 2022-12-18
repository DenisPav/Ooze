namespace Ooze.Typed.Sorters
{
    internal interface IOozeSorterHandler<TEntity, TSorter>
    {
        IQueryable<TEntity> Apply(
            IQueryable<TEntity> query,
            IEnumerable<TSorter> sorters);
    }
}
