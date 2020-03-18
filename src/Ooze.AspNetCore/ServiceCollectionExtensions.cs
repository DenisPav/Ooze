using Microsoft.Extensions.DependencyInjection;
using Ooze.Configuration;
using Ooze.Configuration.Options;
using Ooze.Filters;
using Ooze.Query;
using Ooze.Sorters;
using Ooze.Validation;
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
            var optionsValidator = new OozeOptionsValidator();
            optionsConfigurator ??= _ => { };
            optionsConfigurator(options);

            if (!optionsValidator.Validate(options))
            {
                throw new Exception("Specified option configuration is not valid");
            }

            var configBuilder = new OozeConfigurationBuilder();

            configurationsAssembly.GetTypes()
                .Where(typeof(IOozeConfiguration).IsAssignableFrom)
                .Select(Activator.CreateInstance)
                .Cast<IOozeConfiguration>()
                .ToList()
                .ForEach(configurator => configurator.Configure(configBuilder));

            var configuration = configBuilder.Build(options);

            services.AddSingleton(configuration);
            services.AddScoped<IOozeCustomProviderProvider, OozeCustomProviderProvider>();
            services.AddScoped<IOozeSorterHandler, OozeSorterHandler>();
            services.AddScoped<IOozeFilterHandler, OozeFilterHandler>();
            services.AddScoped<IOozeQueryHandler, OozeQueryHandler>();
            services.AddScoped<IOozeResolver, OozeResolver>();

            return services;
        }
    }
}
