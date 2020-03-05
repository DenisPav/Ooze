using Microsoft.Extensions.DependencyInjection;
using Ooze.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace Ooze.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOoze(
            this IServiceCollection services,
            Assembly configurationsAssembly)
        {
            var configBuilder = new OozeConfigurationBuilder();

            configurationsAssembly.GetTypes()
                .Where(typeof(IOozeConfiguration).IsAssignableFrom)
                .Select(Activator.CreateInstance)
                .Cast<IOozeConfiguration>()
                .ToList()
                .ForEach(configurator => configurator.Configure(configBuilder));

            var configuration = configBuilder.Build();

            services.AddSingleton(configuration);
            services.AddScoped<IOozeResolver, OozeResolver>();

            return services;
        }
    }
}
