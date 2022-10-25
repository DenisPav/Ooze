namespace Ooze.Typed.Sorters
{
    internal interface IOozeSorterHandler<TEntity>
    {
        IQueryable<TEntity> Apply(
            IQueryable<TEntity> query,
            IEnumerable<Sorter> sorters);
    }
}
