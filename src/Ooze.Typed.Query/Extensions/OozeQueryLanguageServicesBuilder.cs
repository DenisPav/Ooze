using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Query.Filters;

namespace Ooze.Typed.Query.Extensions;

/// <inheritdoc />
internal class OozeQueryLanguageServicesBuilder(IServiceCollection services) : IOozeQueryLanguageBuilder
{
    private static readonly Type QueryFilterProviderType = typeof(IQueryLanguageFilterProvider<>);

    public IOozeQueryLanguageBuilder AddQueryProvider<TProvider>(
        ServiceLifetime providerLifetime = ServiceLifetime.Singleton)
    {
        var queryFilterType = typeof(TProvider);
        var implementedInterfaces = queryFilterType.GetInterfaces()
            .Where(@type => type.IsGenericType)
            .ToList();
        var queryFilterProvider = implementedInterfaces.SingleOrDefault(@interface =>
            QueryFilterProviderType.IsAssignableFrom(@interface.GetGenericTypeDefinition()));

        if (queryFilterProvider is not null)
        {
            services.Add(new ServiceDescriptor(queryFilterProvider, queryFilterType, providerLifetime));
        }

        return this;
    }

    internal IOozeQueryLanguageBuilder AddCommonServices()
    {
        services.AddScoped(typeof(IQueryLanguageOperationResolver), typeof(QueryLanguageOperationResolver));
        services.AddScoped(typeof(IQueryLanguageOperationResolver<,,>), typeof(QueryLanguageOperationResolver<,,>));
        services.AddScoped(typeof(IQueryLanguageHandler<>), typeof(QueryLanguageHandler<>));

        return this;
    }
}