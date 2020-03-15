using NSubstitute;
using Ooze.Filters;
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

        public class SampleEntity
        { }

        public class NotUsedEntity 
        { }

        internal class OozeCustomProviderProviderContext
        {
            public OozeCustomProviderProvider OozeCustomProviderProvider { get; set; }
            public IOozeFilterProvider<object> FilterProvider1 { get; } = Substitute.For<IOozeFilterProvider<object>>();
            public IOozeFilterProvider<SampleEntity> FilterProvider2 { get; } = Substitute.For<IOozeFilterProvider<SampleEntity>>();


            public OozeCustomProviderProviderContext()
            {
                OozeCustomProviderProvider = new OozeCustomProviderProvider(new IOozeProvider[] { FilterProvider1, FilterProvider2 });
            }
        }
    }
}
