using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;
using Ooze.Typed.Filters.Async;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;
using Ooze.Typed.Sorters.Async;

namespace Ooze.Typed.Extensions;

/// <inheritdoc />
internal class OozeServiceCollectionBuilder(IServiceCollection services) : IOozeServiceCollectionBuilder
{
    private static readonly Type FilterProviderType = typeof(IFilterProvider<,>);
    private static readonly Type SorterProviderType = typeof(ISorterProvider<,>);
    private static readonly Type AsyncFilterProviderType = typeof(IAsyncFilterProvider<,>);
    private static readonly Type AsyncSorterProviderType = typeof(IAsyncSorterProvider<,>);

    public IOozeServiceCollectionBuilder Add<TProvider>(ServiceLifetime providerLifetime = ServiceLifetime.Singleton)
    {
        var providerType = typeof(TProvider);
        var implementedInterfaces = providerType.GetInterfaces()
            .Where(type => type.IsGenericType)
            .ToList();
        var filterProvider = implementedInterfaces.SingleOrDefault(@interface => CheckTypePredicate(@interface, FilterProviderType));
        var sorterProvider = implementedInterfaces.SingleOrDefault(@interface => CheckTypePredicate(@interface, SorterProviderType));
        var asyncFilterProvider = implementedInterfaces.SingleOrDefault(@interface => CheckTypePredicate(@interface, AsyncFilterProviderType));
        var asyncSorterProvider = implementedInterfaces.SingleOrDefault(@interface => CheckTypePredicate(@interface, AsyncSorterProviderType));

        if (filterProvider is null
            && sorterProvider is null
            && asyncFilterProvider is null
            && asyncSorterProvider is null)
        {
            throw new ArgumentException("Passed Type is not valid Ooze provider", nameof(TProvider));
        }

        if (filterProvider is not null)
        {
            services.Add(new ServiceDescriptor(filterProvider, providerType, providerLifetime));
        }
        if (asyncFilterProvider is not null)
        {
            services.Add(new ServiceDescriptor(asyncFilterProvider, providerType, providerLifetime));
        }
        if (sorterProvider is not null)
        {
            services.Add(new ServiceDescriptor(sorterProvider, providerType, providerLifetime));
        }
        if (asyncSorterProvider is not null)
        {
            services.Add(new ServiceDescriptor(asyncSorterProvider, providerType, providerLifetime));
        }

        return this;
    }

    public IOozeServiceCollectionBuilder EnableAsyncResolvers()
    {
        services.AddScoped<IAsyncOperationResolver, AsyncOperationResolver>();
        services.AddScoped(typeof(IAsyncOperationResolver<,,>), typeof(AsyncOperationResolver<,,>));
        services.AddScoped(typeof(IAsyncFilterHandler<,>), typeof(AsyncFilterHandler<,>));
        services.AddScoped(typeof(IAsyncSorterHandler<,>), typeof(AsyncSorterHandler<,>));

        return this;
    }

    private static bool CheckTypePredicate(Type interfaceType, Type providerType)
        => providerType.IsAssignableFrom(interfaceType.GetGenericTypeDefinition());

    internal IOozeServiceCollectionBuilder AddCommonServices()
    {
        services.AddScoped<IOperationResolver, OperationResolver>();
        services.AddScoped(typeof(IOperationResolver<,,>), typeof(OperationResolver<,,>));
        services.AddScoped(typeof(IFilterHandler<,>), typeof(FilterHandler<,>));
        services.AddScoped(typeof(ISorterHandler<,>), typeof(SorterHandler<,>));
        services.AddScoped(typeof(IPagingHandler<>), typeof(PagingHandler<>));

        return this;
    }
}
