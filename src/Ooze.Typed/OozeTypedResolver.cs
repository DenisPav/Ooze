using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed;

internal class OozeTypedResolver : IOozeTypedResolver
{
    private readonly IServiceProvider _serviceProvider;

    public OozeTypedResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IQueryable<TEntity> Filter<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters filters)
    {
        if (filters is null)
            return query;

        var filterHandler = _serviceProvider.GetRequiredService<IOozeFilterHandler<TEntity, TFilters>>();
        query = filterHandler.Apply(query, filters);

        return query;
    }

    public IQueryable<TEntity> Sort<TEntity, TSorter>(
        IQueryable<TEntity> query,
        IEnumerable<TSorter> sorters)
    {
        if (sorters == null || sorters?.Count() == 0)
            return query;

        var sorterHandler = _serviceProvider.GetRequiredService<IOozeSorterHandler<TEntity, TSorter>>();
        query = sorterHandler.Apply(query, sorters);

        return query;
    }

    public IQueryable<TEntity> Page<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions)
    {
        if (pagingOptions == null)
            return query;

        var sorterHandler = _serviceProvider.GetRequiredService<IOozePagingHandler<TEntity>>();
        query = sorterHandler.Apply(query, pagingOptions);

        return query;
    }
}

internal class OozeTypedResolver<TEntity, TFilters, TSorters> : IOozeTypedResolver<TEntity, TFilters, TSorters>
{
    private readonly IOozeSorterHandler<TEntity, TSorters> _sorterHandler;
    private readonly IOozeFilterHandler<TEntity, TFilters> _filterHandler;
    private readonly IOozePagingHandler<TEntity> _pagingHandler;
    private IQueryable<TEntity> _query;

    public OozeTypedResolver(
        IOozeSorterHandler<TEntity, TSorters> sorterHandler,
        IOozeFilterHandler<TEntity, TFilters> filterHandler,
        IOozePagingHandler<TEntity> pagingHandler)
    {
        _sorterHandler = sorterHandler;
        _filterHandler = filterHandler;
        _pagingHandler = pagingHandler;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query)
    {
        _query = query;
        return this;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> Sort(IEnumerable<TSorters> sorters)
    {
        if (sorters == null || sorters?.Count() == 0)
            return this;

        _query = _sorterHandler.Apply(_query, sorters);
        return this;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> Filter(TFilters filters)
    {
        if (filters is null)
            return this;

        _query = _filterHandler.Apply(_query, filters);
        return this;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> Page(PagingOptions pagingOptions)
    {
        if (pagingOptions == null)
            return this;

        _query = _pagingHandler.Apply(_query, pagingOptions);
        return this;
    }

    public IQueryable<TEntity> Apply()
        => _query;

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters,
        TFilters filters,
        PagingOptions pagingOptions)
    {
        return WithQuery(query)
            .Sort(sorters)
            .Filter(filters)
            .Page(pagingOptions)
            .Apply();
    }
}