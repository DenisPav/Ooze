using Microsoft.Extensions.Logging;

namespace Ooze.Typed.Filters;

internal class FilterHandler<TEntity, TFilters>(
    IEnumerable<IFilterProvider<TEntity, TFilters>> filterProviders,
    ILogger<FilterHandler<TEntity, TFilters>> log)
    : IFilterHandler<TEntity, TFilters>
{
    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        TFilters filters)
    {
        log.LogDebug("Processing available filters!");

        var validFilters = filterProviders.SelectMany(provider => provider.GetFilters())
            .Where(filter => filter.ShouldRun(filters));

        foreach (var filterDefinition in validFilters)
        {
            var filterExpr = filterDefinition.FilterExpressionFactory?.Invoke(filters);
            if (filterExpr is null)
                continue;

            log.LogDebug("Applying filter: [{@filter}]", filterExpr);
            query = query.Where(filterExpr);
        }

        return query;
    }
}