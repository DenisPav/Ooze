using NSubstitute;
using Ooze.Filters;
using Ooze.Sorters;
using System.Linq;
using Xunit;

namespace Ooze.Tests
{
    public class OozeCustomProviderProviderTests
    {
        [Fact]
        public void Should_Correctly_Resolve_Filters()
        {
            var context = new OozeCustomProviderProviderContext();

            Assert.True(context.OozeCustomProviderProvider.FiltersFor<object>().Count() == 1);
            Assert.True(context.OozeCustomProviderProvider.FiltersFor<SampleEntity>().Count() == 1);

            Assert.True(context.OozeCustomProviderProvider.FiltersFor<NotUsedEntity>().Count() == 0);
        }

        [Fact]
        public void Should_Correctly_Resolve_Sorters()
        {
            var context = new OozeCustomProviderProviderContext();

            Assert.True(context.OozeCustomProviderProvider.SortersFor<object>().Count() == 1);
            Assert.True(context.OozeCustomProviderProvider.SortersFor<SampleEntity>().Count() == 1);

            Assert.True(context.OozeCustomProviderProvider.SortersFor<NotUsedEntity>().Count() == 0);
        }

        public class SampleEntity
        { }

        public class NotUsedEntity 
        { }

        internal class OozeCustomProviderProviderContext
        {
            public OozeProviderProvider OozeCustomProviderProvider { get; set; }
            public IOozeFilterProvider<object> FilterProvider1 { get; } = Substitute.For<IOozeFilterProvider<object>>();
            public IOozeFilterProvider<SampleEntity> FilterProvider2 { get; } = Substitute.For<IOozeFilterProvider<SampleEntity>>();
            public IOozeSorterProvider<object> SorterProvider1 { get; } = Substitute.For<IOozeSorterProvider<object>>();
            public IOozeSorterProvider<SampleEntity> SorterProvider2 { get; } = Substitute.For<IOozeSorterProvider<SampleEntity>>();


            public OozeCustomProviderProviderContext()
            {
                OozeCustomProviderProvider = new OozeProviderProvider(new IOozeProvider[] { FilterProvider1, FilterProvider2, SorterProvider1, SorterProvider2 });
            }
        }
    }
}
