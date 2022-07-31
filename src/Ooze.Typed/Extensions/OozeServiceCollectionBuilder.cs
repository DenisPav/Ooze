using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed.Extensions
{
    internal class OozeServiceCollectionBuilder : IOozeServiceCollectionBuilder
    {
        private readonly IServiceCollection _services;

        public OozeServiceCollectionBuilder(IServiceCollection services) => _services = services;

        public IOozeServiceCollectionBuilder Add<TProvider>(ServiceLifetime providerLifetime = ServiceLifetime.Singleton)
        {
            var providerType = typeof(TProvider);
            var implementedInterfaces = providerType.GetInterfaces()
                .Where(@type => type.IsGenericType);
            var filterProvider = implementedInterfaces.SingleOrDefault(@interface => typeof(IOozeFilterProvider<,>).IsAssignableFrom(@interface.GetGenericTypeDefinition()));
            var sorterProvider = implementedInterfaces.SingleOrDefault(@interface => typeof(IOozeSorterProvider<,>).IsAssignableFrom(@interface.GetGenericTypeDefinition()));


            if (filterProvider is not null)
            {
                _services.Add(new ServiceDescriptor(filterProvider, providerType, providerLifetime));
            }
            if (sorterProvider is not null)
            {
                _services.Add(new ServiceDescriptor(sorterProvider, providerType, providerLifetime));
            }

            return this;
        }

        internal IOozeServiceCollectionBuilder AddCommonServices()
        {
            _services.AddScoped<IOozeTypedResolver, OozeTypedResolver>();
            _services.AddScoped(typeof(IOozeTypedResolver<,,>), typeof(OozeTypedResolver<,,>));
            _services.AddScoped(typeof(IOozeFilterHandler<,>), typeof(OozeFilterHandler<,>));
            _services.AddScoped(typeof(IOozeSorterHandler<,>), typeof(OozeSorterHandler<,>));
            _services.AddScoped(typeof(IOozePagingHandler<>), typeof(OozePagingHandler<>));

            return this;
        }
    }
}
