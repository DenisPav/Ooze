using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ooze.Typed.Filters;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed;

/// <inheritdoc />
internal class OperationResolver(
    IServiceProvider serviceProvider,
    ILogger<OperationResolver> log)
    : IOperationResolver
{
    public IQueryable<TEntity> Filter<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters? filters)
    {
        if (filters is null)
        {
            log.LogDebug("Filters of type: [{typeName}] are null", typeof(TFilters).Name);
            return query;
        }

        var filterHandler = serviceProvider.GetRequiredService<IFilterHandler<TEntity, TFilters>>();
        query = filterHandler.Apply(query, filters);

        return query;
    }

    public IQueryable<TEntity> Sort<TEntity, TSorters>(
        IQueryable<TEntity> query,
        IEnumerable<TSorters>? sorters)
    {
        sorters ??= [];
        if (sorters.Any() == false)
        {
            log.LogDebug("Sorters of type: [{typeName}] are not present", typeof(TSorters).Name);
            return query;
        }


        var sorterHandler = serviceProvider.GetRequiredService<ISorterHandler<TEntity, TSorters>>();
        query = sorterHandler.Apply(query, sorters);

        return query;
    }

    public IQueryable<TEntity> Page<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions? pagingOptions)
    {
        if (pagingOptions == null)
        {
            log.LogDebug("Pagination options are not present");
            return query;
        }


        var sorterHandler = serviceProvider.GetRequiredService<IOozePagingHandler<TEntity>>();
        query = sorterHandler.Apply(query, pagingOptions);

        return query;
    }

    public IQueryable<TEntity> PageWithCursor<TEntity, TAfter, TProperty>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions)
    {
        if (pagingOptions == null)
        {
            log.LogDebug("Pagination options are not present");
            return query;
        }

        var sorterHandler = serviceProvider.GetRequiredService<IOozePagingHandler<TEntity>>();
        query = sorterHandler.ApplyCursor(query, cursorPropertyExpression, pagingOptions);

        return query;
    }
}

/// <inheritdoc />
internal class OperationResolver<TEntity, TFilters, TSorters>(
    ISorterHandler<TEntity, TSorters> sorterHandler,
    IFilterHandler<TEntity, TFilters> filterHandler,
    IOozePagingHandler<TEntity> pagingHandler,
    ILogger<OperationResolver<TEntity, TFilters, TSorters>> log)
    : IOperationResolver<TEntity, TFilters, TSorters>
{
    private IQueryable<TEntity> _query = null!;

    public IOperationResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query)
    {
        _query = query;
        return this;
    }

    public IOperationResolver<TEntity, TFilters, TSorters> Sort(IEnumerable<TSorters>? sorters)
    {
        sorters ??= [];
        if (sorters.Any() == false)
        {
            log.LogDebug("Sorters of type: [{typeName}] are not present", typeof(TSorters).Name);
            return this;
        }


        _query = sorterHandler.Apply(_query, sorters);
        return this;
    }

    public IOperationResolver<TEntity, TFilters, TSorters> Filter(TFilters? filters)
    {
        if (filters is null)
        {
            log.LogDebug("Filters of type: [{typeName}] are null", typeof(TFilters).Name);
            return this;
        }


        _query = filterHandler.Apply(_query, filters);
        return this;
    }

    public IOperationResolver<TEntity, TFilters, TSorters> Page(PagingOptions? pagingOptions)
    {
        if (pagingOptions == null)
        {
            log.LogDebug("Pagination options are not present");
            return this;
        }

        _query = pagingHandler.Apply(_query, pagingOptions);
        return this;
    }

    public IOperationResolver<TEntity, TFilters, TSorters> PageWithCursor<TAfter, TProperty>(
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions)
    {
        if (pagingOptions == null)
        {
            log.LogDebug("Pagination options are not present");
            return this;
        }

        _query = pagingHandler.ApplyCursor(_query, cursorPropertyExpression, pagingOptions);
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

    public IQueryable<TEntity> Apply<TAfter, TProperty>(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters,
        TFilters filters,
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions)
    {
        return WithQuery(query)
            .Sort(sorters)
            .Filter(filters)
            .PageWithCursor(cursorPropertyExpression, pagingOptions)
            .Apply();
    }
}