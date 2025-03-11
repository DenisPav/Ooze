using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Query.Filters;

namespace Ooze.Typed.Query.Extensions;

internal class QueryServicesBuilder : IQueryBuilder
{
    private static readonly Type QueryFilterProviderType = typeof(IQueryFilterProvider<>);
    private readonly IServiceCollection _services;

    public QueryServicesBuilder(IServiceCollection services)
    {
        _services = services;
        _services.AddScoped(typeof(IQueryHandler<>), typeof(QueryHandler<>));
    }
    
    public IQueryBuilder AddFilterProvider<TProvider>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var queryFilterType = typeof(TProvider);
        var implementedInterfaces = queryFilterType.GetInterfaces()
            .Where(@type => type.IsGenericType)
            .ToList();
        var queryFilterProvider = implementedInterfaces.SingleOrDefault(@interface =>
            QueryFilterProviderType.IsAssignableFrom(@interface.GetGenericTypeDefinition()));
        
        if (queryFilterProvider is not null)
        {
            _services.Add(new ServiceDescriptor(queryFilterProvider, queryFilterType, lifetime));
        }
        
        return this;
    }
}