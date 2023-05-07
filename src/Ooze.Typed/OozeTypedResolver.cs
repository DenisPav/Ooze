using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ooze.Typed.Filters;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed;

internal class OozeTypedResolver : IOozeTypedResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OozeTypedResolver> _log;

    public OozeTypedResolver(
        IServiceProvider serviceProvider,
        ILogger<OozeTypedResolver> log)
    {
        _serviceProvider = serviceProvider;
        _log = log;
    }

    public IQueryable<TEntity> Filter<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters? filters)
    {
        if (filters is null)
        {
            _log.LogDebug("Filters of type: [{typeName}] are null", typeof(TFilters).Name);
            return query;
        }

        var filterHandler = _serviceProvider.GetRequiredService<IOozeFilterHandler<TEntity, TFilters>>();
        query = filterHandler.Apply(query, filters);

        return query;
    }

    public IQueryable<TEntity> Sort<TEntity, TSorters>(
        IQueryable<TEntity> query,
        IEnumerable<TSorters>? sorters)
    {
        sorters ??= Enumerable.Empty<TSorters>();
        if (sorters.Any() == false)
        {
            _log.LogDebug("Sorters of type: [{typeName}] are not present", typeof(TSorters).Name);
            return query;
        }


        var sorterHandler = _serviceProvider.GetRequiredService<IOozeSorterHandler<TEntity, TSorters>>();
        query = sorterHandler.Apply(query, sorters);

        return query;
    }

    public IQueryable<TEntity> Page<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions? pagingOptions)
    {
        if (pagingOptions == null)
        {
            _log.LogDebug("Pagination options are not present");
            return query;
        }


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
    private readonly ILogger<OozeTypedResolver<TEntity, TFilters, TSorters>> _log;

    private IQueryable<TEntity> _query = null!;

    public OozeTypedResolver(
        IOozeSorterHandler<TEntity, TSorters> sorterHandler,
        IOozeFilterHandler<TEntity, TFilters> filterHandler,
        IOozePagingHandler<TEntity> pagingHandler,
        ILogger<OozeTypedResolver<TEntity, TFilters, TSorters>> log)
    {
        _sorterHandler = sorterHandler;
        _filterHandler = filterHandler;
        _pagingHandler = pagingHandler;
        _log = log;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query)
    {
        _query = query;
        return this;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> Sort(IEnumerable<TSorters>? sorters)
    {
        sorters ??= Enumerable.Empty<TSorters>();
        if (sorters.Any() == false)
        {
            _log.LogDebug("Sorters of type: [{typeName}] are not present", typeof(TSorters).Name);
            return this;
        }


        _query = _sorterHandler.Apply(_query, sorters);
        return this;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> Filter(TFilters? filters)
    {
        if (filters is null)
        {
            _log.LogDebug("Filters of type: [{typeName}] are null", typeof(TFilters).Name);
            return this;
        }


        _query = _filterHandler.Apply(_query, filters);
        return this;
    }

    public IOozeTypedResolver<TEntity, TFilters, TSorters> Page(PagingOptions? pagingOptions)
    {
        if (pagingOptions == null)
        {
            _log.LogDebug("Pagination options are not present");
            return this;
        }

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