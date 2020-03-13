using Microsoft.Extensions.DependencyInjection;
using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Sorters;
using System;
using System.Linq;
using System.Reflection;

namespace Ooze.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOoze(
            this IServiceCollection services,
            Assembly configurationsAssembly,
            Action<OozeOptions> optionsConfigurator = null)
        {
            var options = new OozeOptions();
            optionsConfigurator ??= _ => { };
            optionsConfigurator(options);

            var configBuilder = new OozeConfigurationBuilder();

            configurationsAssembly.GetTypes()
                .Where(typeof(IOozeConfiguration).IsAssignableFrom)
                .Select(Activator.CreateInstance)
                .Cast<IOozeConfiguration>()
                .ToList()
                .ForEach(configurator => configurator.Configure(configBuilder));

            var configuration = configBuilder.Build(options);

            services.AddSingleton(configuration);
            services.AddScoped<IOozeFilterHandler, OozeFilterHandler>();
            services.AddScoped<IOozeSorterHandler, OozeSorterHandler>();
            services.AddScoped<IOozeResolver, OozeResolver>();

            return services;
        }
    }
}
