using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Filters;

namespace Ooze.Typed.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOozeTyped(this IServiceCollection services)
        {
            services.AddScoped<IOozeTypedResolver, OozeTypedResolver>();
            services.AddScoped(typeof(IOozeFilterHandler<,>), typeof(OozeFilterHandler<,>));

            return services;
        }
    }
}
