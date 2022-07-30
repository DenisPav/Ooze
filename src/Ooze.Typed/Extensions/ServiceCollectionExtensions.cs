using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;
using Ooze.Typed.Sorters;

namespace Ooze.Typed.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOozeTyped(this IServiceCollection services)
        {
            services.AddScoped<IOozeTypedResolver, OozeTypedResolver>();
            services.AddScoped(typeof(IOozeFilterHandler<,>), typeof(OozeFilterHandler<,>));
            services.AddScoped(typeof(IOozeSorterHandler<,>), typeof(OozeSorterHandler<,>));

            return services;
        }
    }
}
