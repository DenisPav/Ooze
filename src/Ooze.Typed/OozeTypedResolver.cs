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
        var filterHandler = _serviceProvider.GetRequiredService<IOozeFilterHandler<TEntity, TFilters>>();
        query = filterHandler.Apply(query, filters);

        return query;
    }

    public IQueryable<TEntity> Sort<TEntity, TSorters>(
        IQueryable<TEntity> query,
        TSorters sorters)
    {
        var sorterHandler = _serviceProvider.GetRequiredService<IOozeSorterHandler<TEntity, TSorters>>();
        query = sorterHandler.Apply(query, sorters);

        return query;
    }

    public IQueryable<TEntity> Page<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions)
    {
        var sorterHandler = _serviceProvider.GetRequiredService<IOozePagingHandler<TEntity>>();
        query = sorterHandler.Apply(query, pagingOptions);

        return query;
    }
}

internal class OozeTypedResolver<TEntity, TFilters, TSorters> : IOozeTypedResolver<TEntity, TFilters, TSorters>
{
    private readonly IOozeFilterHandler<TEntity, TFilters> _filterHandler;
    private readonly IOozeSorterHandler<TEntity, TSorters> _sorterHandler;
    private readonly IOozePagingHandler<TEntity> _pagingHandler;
    private IQueryable<TEntity> _query = null;

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

    public IOozeTypedResolver<TEntity, TFilters, TSorters> Sort(TSorters sorters)
    {
        _query = _sorterHandler.Apply(_query, sorters);
        return this;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> Filter(TFilters filters)
    {
        _query = _filterHandler.Apply(_query, filters);
        return this;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> Page(PagingOptions pagingOptions)
    {
        _query = _pagingHandler.Apply(_query, pagingOptions);
        return this;
    }

    public IQueryable<TEntity> Apply() 
        => _query;

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        TSorters sorters,
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
