namespace Ooze.Typed.Sorters
{
    internal interface IOozeSorterHandler<TEntity, TSorters>
    {
        IQueryable<TEntity> Apply(
            IQueryable<TEntity> query,
            TSorters sorters);
    }
}
