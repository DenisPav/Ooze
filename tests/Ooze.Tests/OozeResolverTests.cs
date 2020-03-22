using Xunit;
using NSubstitute;
using Ooze.Filters;
using Ooze.Sorters;
using System.Linq;
using Ooze.Configuration;
using Ooze.Configuration.Options;
using System.Collections.Generic;
using System;
using Ooze.Query;

namespace Ooze.Tests
{
    public class OozeResolverTests
    {
        [Fact]
        public void Should_Call_Filter_Sorter_Handler_If_Model_Is_Valid()
        {
            var context = new OozeResolverContext();
            var model = new OozeModel
            {
                Filters = "-",
                Sorters = "-"
            };

            var data = Enumerable.Empty<object>().AsQueryable();
            context.OozeResolver.Apply(data, model);

            context.FilterHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Filters);
            context.SorterHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Sorters);
            context.QueryHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Sorters);
        }

        [Fact]
        public void Should_Call_Query_Handler_If_Model_Is_Valid()
        {
            var context = new OozeResolverContext();
            var model = new OozeModel
            {
                Filters = "-",
                Sorters = "-",
                Query = "-"
            };

            var data = Enumerable.Empty<object>().AsQueryable();
            context.OozeResolver.Apply(data, model);

            context.FilterHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Filters);
            context.SorterHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Sorters);
            context.QueryHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Sorters);
        }

        [Theory]
        [InlineData("sorter", "")]
        [InlineData("", "filter")]
        [InlineData("sorter", null)]
        [InlineData(null, "filter")]
        [InlineData(null, null)]
        public void Should_Not_Call_Handlers_If_Any_Model_Property_Is_Invalid(string sorter, string filter)
        {
            var context = new OozeResolverContext();
            var model = new OozeModel
            {
                Filters = filter,
                Sorters = sorter
            };

            var data = Enumerable.Empty<object>().AsQueryable();
            context.OozeResolver.Apply(data, model);

            if (string.IsNullOrEmpty(sorter))
            {
                context.SorterHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Sorters);
            }
            else
            {
                context.SorterHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Sorters);
            }

            if (string.IsNullOrEmpty(filter))
            {
                context.FilterHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Filters);
            }
            else
            {
                context.FilterHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Filters);
            }
        }

        internal class OozeResolverContext
        {
            public OozeResolver OozeResolver { get; private set; }
            public IOozeFilterHandler FilterHandler { get; } = Substitute.For<IOozeFilterHandler>();
            public IOozeSorterHandler SorterHandler { get; } = Substitute.For<IOozeSorterHandler>();
            public IOozeQueryHandler QueryHandler { get; } = Substitute.For<IOozeQueryHandler>();
            public OozeConfiguration Configuration { get; } = new OozeConfiguration(new OozeOptions())
            {
                EntityConfigurations = new Dictionary<Type, OozeEntityConfiguration>
                {
                    { typeof(object), null }
                }
            };

            public OozeResolverContext()
            {
                OozeResolver = new OozeResolver(SorterHandler, FilterHandler, QueryHandler, Configuration);
            }
        }
    }
}