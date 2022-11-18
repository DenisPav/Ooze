using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed;

public interface IOozeTypedResolver
{
    IQueryable<TEntity> Sort<TEntity>(
        IQueryable<TEntity> query,
        IEnumerable<Sorter> sorters);

    IQueryable<TEntity> Filter<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters filters);

    IQueryable<TEntity> Query<TEntity>(
        IQueryable<TEntity> query,
        string queryDefinition);

    IQueryable<TEntity> Page<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions);
}

public interface IOozeTypedResolver<TEntity, TFilters>
{
    IOozeTypedResolver<TEntity, TFilters> WithQuery(IQueryable<TEntity> query);
    IOozeTypedResolver<TEntity, TFilters> Sort(IEnumerable<Sorter> sorters);
    IOozeTypedResolver<TEntity, TFilters> Filter(TFilters filters);
    IQueryable<TEntity> Query(string queryDefinition);
    IOozeTypedResolver<TEntity, TFilters> Page(PagingOptions pagingOptions);
    IQueryable<TEntity> Apply();

    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<Sorter> sorters,
        TFilters filters,
        PagingOptions pagingOptions);
}