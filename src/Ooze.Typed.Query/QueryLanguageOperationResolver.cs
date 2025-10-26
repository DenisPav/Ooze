using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ooze.Typed.Paging;

namespace Ooze.Typed.Query;

/// <inheritdoc />
internal class QueryLanguageOperationResolver(
    IOperationResolver rootOperationResolver,
    IServiceProvider serviceProvider,
    ILogger<QueryLanguageOperationResolver> log) : IQueryLanguageOperationResolver
{
    public IQueryable<TEntity> Sort<TEntity, TSorter>(
        IQueryable<TEntity> query,
        IEnumerable<TSorter> sorters)
        => rootOperationResolver.Sort(query, sorters);

    public IQueryable<TEntity> Filter<TEntity, TFilters>(
        IQueryable<TEntity> query,
        TFilters filters)
        => rootOperationResolver.Filter(query, filters);

    public IQueryable<TEntity> Page<TEntity>(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions)
        => rootOperationResolver.Page(query, pagingOptions);

    public IQueryable<TEntity> PageWithCursor<TEntity, TAfter, TProperty>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions)
        => rootOperationResolver.PageWithCursor(query, cursorPropertyExpression, pagingOptions);

    public IQueryable<TEntity> FilterWithQueryLanguage<TEntity>(
        IQueryable<TEntity> queryable,
        string? query)
    {
        if (string.IsNullOrEmpty(query))
        {
            log.LogDebug("Query is null or empty");
            return queryable;
        }

        var queryHandler = serviceProvider.GetRequiredService<IQueryLanguageHandler<TEntity>>();
        queryable = queryHandler.Apply(queryable, query);

        return queryable;
    }
}

/// <inheritdoc />
internal class QueryLanguageOperationResolver<TEntity, TFilters, TSorters>(
    IOperationResolver<TEntity, TFilters, TSorters> rootOperationResolver,
    IQueryLanguageHandler<TEntity> queryHandler,
    ILogger<QueryLanguageOperationResolver<TEntity, TFilters, TSorters>> log)
    : IQueryLanguageOperationResolver<TEntity, TFilters, TSorters>
{
    public IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> WithQuery(IQueryable<TEntity> query)
    {
        rootOperationResolver.WithQuery(query);
        return this;
    }

    public IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> Sort(IEnumerable<TSorters> sorters)
    {
        rootOperationResolver.Sort(sorters);
        return this;
    }

    public IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> Filter(TFilters filters)
    {
        rootOperationResolver.Filter(filters);
        return this;
    }

    public IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> FilterWithQueryLanguage(string? query)
    {
        var queryable = Apply();
        if (string.IsNullOrEmpty(query))
        {
            log.LogDebug("Query is null or empty");
            return this;
        }

        queryable = queryHandler.Apply(queryable, query);
        WithQuery(queryable);
        return this;
    }

    public IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> Page(PagingOptions pagingOptions)
    {
        rootOperationResolver.Page(pagingOptions);
        return this;
    }

    public IQueryLanguageOperationResolver<TEntity, TFilters, TSorters> PageWithCursor<TAfter, TProperty>(
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions)
    {
        rootOperationResolver.PageWithCursor(cursorPropertyExpression, pagingOptions);
        return this;
    }

    public IQueryable<TEntity> Apply()
        => rootOperationResolver.Apply();

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters,
        TFilters filters,
        string queryLanguageQuery,
        PagingOptions pagingOptions)
    {
        rootOperationResolver.WithQuery(query)
            .Sort(sorters)
            .Filter(filters);
        FilterWithQueryLanguage(queryLanguageQuery);

        return rootOperationResolver.Page(pagingOptions)
            .Apply();
    }

    public IQueryable<TEntity> Apply<TAfter, TProperty>(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters,
        TFilters filters,
        string queryLanguageQuery,
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter>? pagingOptions)
    {
        rootOperationResolver.WithQuery(query)
            .Sort(sorters)
            .Filter(filters);
        FilterWithQueryLanguage(queryLanguageQuery);

        return rootOperationResolver.PageWithCursor(cursorPropertyExpression, pagingOptions)
            .Apply();
    }
}