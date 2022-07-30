namespace Ooze.Typed;

public interface IOozeTypedResolver
{
    IQueryable<TEntity> Filter<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters filters);
    IQueryable<TEntity> Sort<TEntity, TSorters>(
        IQueryable<TEntity> query,
        TSorters sorters);
}

public interface IOozeTypedResolver<TEntity, TFilters, TSorters>
{
    IOozeTypedResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query);
    IOozeTypedResolver<TEntity, TFilters, TSorters> Filter(TFilters filters);
    IOozeTypedResolver<TEntity, TFilters, TSorters> Sort(TSorters sorters);
    IQueryable<TEntity> Apply();
}
