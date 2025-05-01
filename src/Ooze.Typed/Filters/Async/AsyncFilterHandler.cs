using Microsoft.Extensions.Logging;

namespace Ooze.Typed.Filters.Async;

internal class AsyncFilterHandler<TEntity, TFilters>(
    IEnumerable<IAsyncFilterProvider<TEntity, TFilters>> filterProviders,
    ILogger<AsyncFilterHandler<TEntity, TFilters>> log)
    : IAsyncFilterHandler<TEntity, TFilters>
{
    public async ValueTask<IQueryable<TEntity>> ApplyAsync(
        IQueryable<TEntity> query,
        TFilters filters)
    {
        log.LogDebug("Processing available filters!");

        var filterDefinitions = new List<AsyncFilterDefinition<TEntity, TFilters>>();
        foreach (var provider in filterProviders)
        {
            var definitions = await provider.GetFiltersAsync()
                .ConfigureAwait(false);
            filterDefinitions.AddRange(definitions);
        }

        foreach (var filterDefinition in filterDefinitions)
        {
            var shouldRun = await filterDefinition.ShouldRun(filters)
                .ConfigureAwait(false);
            if (shouldRun == false)
                continue;

            var filterExpr = await filterDefinition.FilterExpressionFactory
                .Invoke(filters)
                .ConfigureAwait(false);
            if (filterExpr is null)
                continue;

            log.LogDebug("Applying filter: [{@filter}]", filterExpr);
            query = query.Where(filterExpr);
        }

        return query;
    }
}