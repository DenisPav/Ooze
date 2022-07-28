using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;

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
}
