using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed;

public interface IOozeTypedResolver
{
    IQueryable<TEntity> Sort<TEntity, TSorter>(
        IQueryable<TEntity> query,
        IEnumerable<TSorter> sorters);

    IQueryable<TEntity> Filter<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters filters);

    IQueryable<TEntity> Page<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions);
}

public interface IOozeTypedResolver<TEntity, TFilters, TSorters>
{
    IOozeTypedResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query);
    IOozeTypedResolver<TEntity, TFilters, TSorters> Sort(IEnumerable<TSorters> sorters);
    IOozeTypedResolver<TEntity, TFilters, TSorters> Filter(TFilters filters);
    IOozeTypedResolver<TEntity, TFilters, TSorters> Page(PagingOptions pagingOptions);
    IQueryable<TEntity> Apply();

    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters,
        TFilters filters,
        PagingOptions pagingOptions);
}