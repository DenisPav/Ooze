using Ooze.Configuration;
using System;
using System.Linq;
using Xunit;

namespace Ooze.Tests
{
    public class OozeEntityConfigurationBuilderTests
    {
        [Fact]
        public void Should_Contain_Sort_Definitions()
        {
            var context = new OozeEntityConfigurationBuilderContext();
            var builder = context.Create<Entity1>();

            builder
                .Sort(instance => instance.Prop1)
                .Sort("randomname", instance => instance.Prop2)
                .Sort(instance => instance.Prop3)
                .Sort(instance => instance.Prop4);

            var (entityType, configuration) = builder.Build();

            Assert.True(entityType == typeof(Entity1));

            var sorters = configuration.Sorters.ToList();
            Assert.True(sorters[0].Name == nameof(Entity1.Prop1));
            Assert.True(sorters[1].Name == "randomname");
            Assert.True(sorters[2].Name == nameof(Entity1.Prop3));
            Assert.True(sorters[3].Name == nameof(Entity1.Prop4));

            Assert.True(sorters[0].Type == typeof(int));
            Assert.True(sorters[1].Type == typeof(string));
            Assert.True(sorters[2].Type == typeof(bool));

            //TODO: check nullable implementation that probably isn't covered yet
            //Assert.True(sorters[3].Type == typeof(long));
        }

        [Fact]
        public void Should_Contain_Filter_Definitions()
        {
            var context = new OozeEntityConfigurationBuilderContext();
            var builder = context.Create<Entity1>();

            builder
                .Filter(instance => instance.Prop1)
                .Filter("randomname", instance => instance.Prop2)
                .Filter(instance => instance.Prop3)
                .Filter(instance => instance.Prop4);

            var (entityType, configuration) = builder.Build();

            Assert.True(entityType == typeof(Entity1));

            var filters = configuration.Filters.ToList();
            Assert.True(filters[0].Name == nameof(Entity1.Prop1));
            Assert.True(filters[1].Name == "randomname");
            Assert.True(filters[2].Name == nameof(Entity1.Prop3));
            Assert.True(filters[3].Name == nameof(Entity1.Prop4));

            Assert.True(filters[0].Type == typeof(int));
            Assert.True(filters[1].Type == typeof(string));
            Assert.True(filters[2].Type == typeof(bool));

            //TODO: check nullable implementation that probably isn't covered yet
            //Assert.True(sorters[3].Type == typeof(long));
        }

        [Fact]
        public void Should_Fail_If_Not_Member_Expression()
        {
            var context = new OozeEntityConfigurationBuilderContext();
            var builder = context.Create<Entity1>();

            Assert.Throws<Exception>(() => builder.Sort<object>(instance => instance.Prop1));
            Assert.Throws<Exception>(() => builder.Sort<object>("name", instance => instance.Prop1));

            Assert.Throws<Exception>(() => builder.Filter<object>(instance => instance.Prop1));
            Assert.Throws<Exception>(() => builder.Filter<object>("name", instance => instance.Prop1));
        }

        internal class Entity1
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public bool Prop3 { get; set; }
            public long? Prop4 { get; set; }
        }

        internal class OozeEntityConfigurationBuilderContext
        {
            public OozeEntityConfigurationBuilder<TEntity> Create<TEntity>()
                where TEntity : class
                => OozeEntityConfigurationBuilder<TEntity>.Create();
        }
    }
}
