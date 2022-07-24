namespace Ooze.Typed.Filters;

internal class OozeFilterHandler<TEntity, TFilter> : IOozeFilterHandler<TEntity, TFilter>
{
    private readonly IEnumerable<IOozeFilterProvider<TEntity, TFilter>> _filterProviders;

    public OozeFilterHandler(
        IEnumerable<IOozeFilterProvider<TEntity, TFilter>> filterProviders)
    {
        _filterProviders = filterProviders;
    }

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        TFilter filters)
    {
        var validFilters = _filterProviders.SelectMany(provider => provider.GetFilters())
            .Cast<FilterDefinition<TEntity, TFilter>>()
            .Where(filter => filter.ShouldRun(filters));

        foreach (var filterDefinition in validFilters)
        {
            var filterExpr = filterDefinition.FilterExpressionFactory?.Invoke(filters);
            if (filterExpr is not null)
            {
                query = query.Where(filterExpr);
            }
        }

        return query;
    }
}
