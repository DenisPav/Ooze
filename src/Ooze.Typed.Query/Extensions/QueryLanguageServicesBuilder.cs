using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Query.Filters;

namespace Ooze.Typed.Query.Extensions;

internal class QueryLanguageServicesBuilder(IServiceCollection services) : IQueryLanguageBuilder
{
    private static readonly Type QueryFilterProviderType = typeof(IQueryLanguageFilterProvider<>);

    public IQueryLanguageBuilder AddQueryProvider<TProvider>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
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

    internal IQueryLanguageBuilder AddCommonServices()
    {
        services.AddScoped(typeof(IQueryLanguageOperationResolver), typeof(QueryLanguageOperationResolver));
        services.AddScoped(typeof(IQueryLanguageOperationResolver<,,>), typeof(QueryLanguageOperationResolver<,,>));
        services.AddScoped(typeof(IQueryLanguageHandler<>), typeof(QueryLanguageHandler<>));

        return this;
    }
}