using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Query.Filters;

namespace Ooze.Typed.Query.Extensions;

internal class QueryServicesBuilder(IServiceCollection services) : IQueryBuilder
{
    private static readonly Type QueryFilterProviderType = typeof(IQueryFilterProvider<>);

    public IQueryBuilder AddQueryProvider<TProvider>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var queryFilterType = typeof(TProvider);
        var implementedInterfaces = queryFilterType.GetInterfaces()
            .Where(@type => type.IsGenericType)
            .ToList();
        var queryFilterProvider = implementedInterfaces.SingleOrDefault(@interface =>
            QueryFilterProviderType.IsAssignableFrom(@interface.GetGenericTypeDefinition()));
        
        if (queryFilterProvider is not null)
        {
            services.Add(new ServiceDescriptor(queryFilterProvider, queryFilterType, lifetime));
        }
        
        return this;
    }

    internal IQueryBuilder AddCommonServices()
    {
        services.AddScoped(typeof(IQueryOperationResolver), typeof(QueryOperationResolver));
        services.AddScoped(typeof(IQueryOperationResolver<,,>), typeof(QueryOperationResolver<,,>));
        services.AddScoped(typeof(IQueryHandler<>), typeof(QueryHandler<>));

        return this;
    }
}