using Ooze.Configuration;
using System;
using System.Linq;
using Xunit;

namespace Ooze.Tests
{
    public class OozeConfigurationBuilderTests
    {
        [Fact]
        public void Should_Contain_Valid_Number_Of_Configurations()
        {
            var context = new OozeConfigurationBuilderContext();
            context.ConfigurationBuilder
                .Entity<Entity1>();

            context.ConfigurationBuilder
                .Entity<Entity2>();

            context.ConfigurationBuilder
                .Entity<Entity3>();

            var configuration = context.ConfigurationBuilder
                .Build(new OozeOptions());

            Assert.True(configuration.EntityConfigurations.Count() == 3);

            Assert.True(configuration.EntityConfigurations.ContainsKey(typeof(Entity1)));
            Assert.True(configuration.EntityConfigurations.ContainsKey(typeof(Entity2)));
            Assert.True(configuration.EntityConfigurations.ContainsKey(typeof(Entity3)));
        }

        [Fact]
        public void Should_Throw_When_Same_Entity_Is_Registered()
        {
            var context = new OozeConfigurationBuilderContext();
            context.ConfigurationBuilder
                .Entity<Entity1>();

            context.ConfigurationBuilder
                .Entity<Entity2>();

            context.ConfigurationBuilder
                .Entity<Entity3>();

            context.ConfigurationBuilder
                .Entity<Entity3>();

            Assert.Throws<ArgumentException>(() => context.ConfigurationBuilder.Build(new OozeOptions()));
        }

        class Entity1 { }
        class Entity2 { }
        class Entity3 { }

        class OozeConfigurationBuilderContext
        {
            public OozeConfigurationBuilder ConfigurationBuilder { get; set; }

            public OozeConfigurationBuilderContext()
            {
                ConfigurationBuilder = new OozeConfigurationBuilder();
            }
        }
    }
}
