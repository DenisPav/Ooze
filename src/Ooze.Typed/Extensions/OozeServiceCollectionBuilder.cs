using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed.Extensions;

/// <inheritdoc />
internal class OozeServiceCollectionBuilder(IServiceCollection services) : IOozeServiceCollectionBuilder
{
    private static readonly Type FilterProviderType = typeof(IOozeFilterProvider<,>);
    private static readonly Type SorterProviderType = typeof(IOozeSorterProvider<,>);

    public IOozeServiceCollectionBuilder Add<TProvider>(ServiceLifetime providerLifetime = ServiceLifetime.Singleton)
    {
        var providerType = typeof(TProvider);
        var implementedInterfaces = providerType.GetInterfaces()
            .Where(type => type.IsGenericType)
            .ToList();
        var filterProvider = implementedInterfaces.SingleOrDefault(@interface => CheckTypePredicate(@interface, FilterProviderType));
        var sorterProvider = implementedInterfaces.SingleOrDefault(@interface => CheckTypePredicate(@interface, SorterProviderType));

        if (filterProvider is null && sorterProvider is null)
        {
            throw new ArgumentException("Passed Type is not valid Ooze provider", nameof(TProvider));
        }

        if (filterProvider is not null)
        {
            services.Add(new ServiceDescriptor(filterProvider, providerType, providerLifetime));
        }
        if (sorterProvider is not null)
        {
            services.Add(new ServiceDescriptor(sorterProvider, providerType, providerLifetime));
        }

        return this;
    }

    private static bool CheckTypePredicate(Type interfaceType, Type providerType)
        => providerType.IsAssignableFrom(interfaceType.GetGenericTypeDefinition());

    internal IOozeServiceCollectionBuilder AddCommonServices()
    {
        services.AddScoped<IOozeTypedResolver, OozeTypedResolver>();
        services.AddScoped(typeof(IOozeTypedResolver<,,>), typeof(OozeTypedResolver<,,>));
        services.AddScoped(typeof(IOozeFilterHandler<,>), typeof(OozeFilterHandler<,>));
        services.AddScoped(typeof(IOozeSorterHandler<,>), typeof(OozeSorterHandler<,>));
        services.AddScoped(typeof(IOozePagingHandler<>), typeof(OozePagingHandler<>));

        return this;
    }
}
