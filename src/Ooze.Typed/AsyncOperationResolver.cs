using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ooze.Typed.Filters.Async;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters.Async;

namespace Ooze.Typed;

/// <inheritdoc />
internal class AsyncOperationResolver(
    IServiceProvider serviceProvider,
    ILogger<AsyncOperationResolver> log)
    : IAsyncOperationResolver
{
    public async ValueTask<IQueryable<TEntity>> FilterAsync<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters? filters)
    {
        if (filters is null)
        {
            log.LogDebug("Filters of type: [{typeName}] are null", typeof(TFilters).Name);
            return query;
        }

        var filterHandler = serviceProvider.GetRequiredService<IAsyncFilterHandler<TEntity, TFilters>>();
        query = await filterHandler.ApplyAsync(query, filters)
            .ConfigureAwait(false);

        return query;
    }

    public async ValueTask<IQueryable<TEntity>> SortAsync<TEntity, TSorters>(
        IQueryable<TEntity> query,
        IEnumerable<TSorters>? sorters)
    {
        sorters ??= [];
        if (sorters.Any() == false)
        {
            log.LogDebug("Sorters of type: [{typeName}] are not present", typeof(TSorters).Name);
            return query;
        }

        var sorterHandler = serviceProvider.GetRequiredService<IAsyncSorterHandler<TEntity, TSorters>>();
        query = await sorterHandler.ApplyAsync(query, sorters)
            .ConfigureAwait(false);

        return query;
    }

    public ValueTask<IQueryable<TEntity>> PageAsync<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions? pagingOptions)
    {
        if (pagingOptions == null)
        {
            log.LogDebug("Pagination options are not present");
            return ValueTask.FromResult(query);
        }
        
        var pagingHandler = serviceProvider.GetRequiredService<IOozePagingHandler<TEntity>>();
        query = pagingHandler.Apply(query, pagingOptions);

        return ValueTask.FromResult(query);
    }

    public ValueTask<IQueryable<TEntity>> PageWithCursorAsync<TEntity, TAfter, TProperty>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions)
    {
        if (pagingOptions == null)
        {
            log.LogDebug("Pagination options are not present");
            return ValueTask.FromResult(query);
        }

        var sorterHandler = serviceProvider.GetRequiredService<IOozePagingHandler<TEntity>>();
        query = sorterHandler.ApplyCursor(query, cursorPropertyExpression, pagingOptions);

        return ValueTask.FromResult(query);
    }
}

/// <inheritdoc />
internal class AsyncOperationResolver<TEntity, TFilters, TSorters>(
    IAsyncSorterHandler<TEntity, TSorters> sorterHandler,
    IAsyncFilterHandler<TEntity, TFilters> filterHandler,
    IOozePagingHandler<TEntity> pagingHandler,
    ILogger<AsyncOperationResolver<TEntity, TFilters, TSorters>> log)
    : IAsyncOperationResolver<TEntity, TFilters, TSorters>
{
    private OozeResolverData<TEntity, TFilters, TSorters> _resolverData = new();

    public IAsyncOperationResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query)
    {
        _resolverData = _resolverData with
        {
            Query = query
        };
        
        return this;
    }

    public IAsyncOperationResolver<TEntity, TFilters, TSorters> Sort(IEnumerable<TSorters>? sorters)
    {
        _resolverData = _resolverData with
        {
            Sorters = sorters
        };
        
        return this;
    }

    public IAsyncOperationResolver<TEntity, TFilters, TSorters> Filter(TFilters? filters)
    {
        _resolverData = _resolverData with
        {
            Filters = filters
        };
        
        return this;
    }

    public IAsyncOperationResolver<TEntity, TFilters, TSorters> Page(PagingOptions? pagingOptions)
    {
        _resolverData = _resolverData with
        {
            Paging = pagingOptions
        };
        
        return this;
    }

    public async ValueTask<IQueryable<TEntity>> ApplyAsync()
    {
        if (_resolverData.Query is null)
            throw new Exception("Queryable not defined/passed to the resolver!");

        if (_resolverData.Sorters is { } sorters)
        {
            log.LogDebug("Applying sorters: [{typeName}]", typeof(TSorters).Name);

            _resolverData = _resolverData with
            {
                Query = await sorterHandler.ApplyAsync(_resolverData.Query, sorters)
                    .ConfigureAwait(false)
            };
        }

        if (_resolverData.Filters is { } filters)
        {
            log.LogDebug("Applying filters: [{typeName}]", typeof(TFilters).Name);
            
            _resolverData = _resolverData with
            {
                Query = await filterHandler.ApplyAsync(_resolverData.Query, filters)
                    .ConfigureAwait(false)
            };
        }

        if (_resolverData.Paging is { } paging)
        {
            log.LogDebug("Applying pagination options");
            
            _resolverData = _resolverData with
            {
                Query = pagingHandler.Apply(_resolverData.Query, paging)
            };
        }
            

        return _resolverData.Query;
    }
}

internal record OozeResolverData<TEntity, TFilter, TSorter>(
     IQueryable<TEntity>? Query = default,
     TFilter? Filters = default,
     IEnumerable<TSorter>? Sorters = default,
     PagingOptions? Paging = default);