using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;
using Ooze.Typed.Query.Filters;

namespace Ooze.Typed.Query.Extensions;

public static class OozeQueryServiceCollectionExtensions
{
    private static readonly Type QueryFilterProviderType = typeof(IOozeQueryFilterProvider<>);

    public static IOozeServiceCollectionBuilder AddQueryHandler(this IOozeServiceCollectionBuilder oozeBuilder)
        => oozeBuilder.AddQueryHandler(typeof(OozeQueryHandler<>));

    public static IOozeServiceCollectionBuilder AddQueryFilter<TQueryFilter>(
        this IOozeServiceCollectionBuilder oozeBuilder,
        ServiceLifetime providerLifetime = ServiceLifetime.Singleton)
    {
        var queryFilterType = typeof(TQueryFilter);
        var implementedInterfaces = queryFilterType.GetInterfaces()
            .Where(@type => type.IsGenericType)
            .ToList();
        var queryFilterProvider = implementedInterfaces.SingleOrDefault(@interface =>
            QueryFilterProviderType.IsAssignableFrom(@interface.GetGenericTypeDefinition()));

        if (queryFilterProvider is not null)
        {
            oozeBuilder.Services.Add(new ServiceDescriptor(queryFilterProvider, queryFilterType, providerLifetime));
        }

        return oozeBuilder;
    }
}