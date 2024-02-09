using Microsoft.Extensions.Logging;

namespace Ooze.Typed.Filters;

internal class OozeFilterHandler<TEntity, TFilters>(
    IEnumerable<IOozeFilterProvider<TEntity, TFilters>> filterProviders,
    ILogger<OozeFilterHandler<TEntity, TFilters>> log)
    : IOozeFilterHandler<TEntity, TFilters>
{
    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        TFilters filters)
    {
        log.LogDebug("Processing available filters!");

        var validFilters = filterProviders.SelectMany(provider => provider.GetFilters())
            .Cast<FilterDefinition<TEntity, TFilters>>()
            .Where(filter => filter.ShouldRun(filters));

        foreach (var filterDefinition in validFilters)
        {
            var filterExpr = filterDefinition.FilterExpressionFactory?.Invoke(filters);
            if (filterExpr is not null)
            {
                log.LogDebug("Applying filter: [{@filter}]", filterExpr);
                query = query.Where(filterExpr);
            }
        }

        return query;
    }
}
