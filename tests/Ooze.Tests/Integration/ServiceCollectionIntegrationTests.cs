using Xunit;
using Ooze.AspNetCore;
using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Sorters;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using Ooze.Query;

namespace Ooze.Tests.Integration
{
    public class ServiceCollectionIntegrationTests
    {
        public IServiceCollection Services => new ServiceCollection();

        [Fact]
        public void Should_Have_Registered_Services()
        {
            var services = Services
                .AddOoze(typeof(ServiceCollectionIntegrationTests).Assembly);

            var requiredTypes = new[]
            {
                (typeof(OozeConfiguration), ServiceLifetime.Singleton),
                (typeof(IOozeFilterHandler), ServiceLifetime.Scoped),
                (typeof(IOozeSorterHandler), ServiceLifetime.Scoped),
                (typeof(IOozeQueryHandler), ServiceLifetime.Scoped),
                (typeof(IOozeResolver), ServiceLifetime.Scoped)
            };

            foreach (var type in requiredTypes)
            {
                var count = services
                    .Count(descriptor => descriptor.ServiceType == type.Item1 && descriptor.Lifetime == type.Item2);

                Assert.True(count == 1);
            }
        }

        [Fact]
        public void Should_Throw_If_Options_Are_Not_Valid()
        {
            Assert.ThrowsAny<Exception>(() => Services
               .AddOoze(
                    typeof(ServiceCollectionIntegrationTests).Assembly,
                    opts =>
                    {
                        opts.Operations.Contains = "dasd";
                    }));
        }

        delegate void AssertDelegate(Type type, IServiceProvider provider);

        [Fact]
        public void Should_Be_Able_To_Resolve_Registered_Services()
        {
            var services = Services.AddOoze(typeof(ServiceCollectionIntegrationTests).Assembly);
            var serviceProvider = new DefaultServiceProviderFactory(
                new ServiceProviderOptions
                {
                    ValidateScopes = true
                }).CreateServiceProvider(services);

            void AssertNoScope(Type type, IServiceProvider provider) => Assert.NotNull(provider.GetRequiredService(type));
            void AssertScope(Type type, IServiceProvider provider)
            {
                using var scope = provider.CreateScope();
                Assert.NotNull(scope.ServiceProvider.GetRequiredService(type));
            };

            var requiredTypes = new (Type, AssertDelegate)[]
            {
                (typeof(OozeConfiguration), AssertNoScope),
                (typeof(IOozeFilterHandler), AssertScope),
                (typeof(IOozeSorterHandler), AssertScope),
                (typeof(IOozeQueryHandler), AssertScope),
                (typeof(IOozeResolver), AssertScope)
            };

            foreach (var definition in requiredTypes)
            {
                definition.Item2(definition.Item1, serviceProvider);
            }
        }
    }
}
