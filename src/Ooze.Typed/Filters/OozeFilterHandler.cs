using Microsoft.Extensions.Logging;

namespace Ooze.Typed.Filters;

internal class OozeFilterHandler<TEntity, TFilters> : IOozeFilterHandler<TEntity, TFilters>
{
    private readonly IEnumerable<IOozeFilterProvider<TEntity, TFilters>> _filterProviders;
    private readonly ILogger<OozeFilterHandler<TEntity, TFilters>> _log;

    public OozeFilterHandler(
        IEnumerable<IOozeFilterProvider<TEntity, TFilters>> filterProviders,
        ILogger<OozeFilterHandler<TEntity, TFilters>> log)
    {
        _filterProviders = filterProviders;
        _log = log;
    }

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        TFilters filters)
    {
        _log.LogDebug("Processing available filters!");

        var validFilters = _filterProviders.SelectMany(provider => provider.GetFilters())
            .Cast<FilterDefinition<TEntity, TFilters>>()
            .Where(filter => filter.ShouldRun(filters));

        foreach (var filterDefinition in validFilters)
        {
            var filterExpr = filterDefinition.FilterExpressionFactory?.Invoke(filters);
            if (filterExpr is not null)
            {
                _log.LogDebug("Applying filter: [{@filter}]", filterExpr);
                query = query.Where(filterExpr);
            }
        }

        return query;
    }
}
