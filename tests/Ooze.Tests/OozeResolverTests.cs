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
using Ooze.Selections;

namespace Ooze.Tests
{
    public class OozeResolverTests
    {
        [Fact]
        public void Should_Call_Filter_Sorter_Selection_Handler_If_Model_Is_Valid()
        {
            var context = new OozeResolverContext();
            var model = new OozeModel
            {
                Filters = "-",
                Sorters = "-",
                Fields = "-"
            };

            var data = Enumerable.Empty<object>().AsQueryable();
            context.OozeResolver.Apply(data, model);

            context.FilterHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Filters);
            context.SorterHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Sorters);
            context.SelectionHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Fields);
            context.QueryHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Query);
        }

        [Fact]
        public void Should_Call_Query_Handler_If_Model_Is_Valid()
        {
            var context = new OozeResolverContext();
            var model = new OozeModel
            {
                Filters = "-",
                Sorters = "-",
                Query = "-",
                Fields = "-"
            };

            var data = Enumerable.Empty<object>().AsQueryable();
            context.OozeResolver.Apply(data, model);

            context.FilterHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Filters);
            context.SorterHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Sorters);
            context.SelectionHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Fields);
            context.QueryHandler.Received().Handle(Arg.Any<IQueryable<object>>(), model.Query);
        }

        [Fact]
        public void Should_Not_Call_Handlers_If_Any_Model_Property_Is_Invalid()
        {
            var context = new OozeResolverContext();
            var model = new OozeModel
            { };

            var data = Enumerable.Empty<object>().AsQueryable();
            context.OozeResolver.Apply(data, model);

            context.FilterHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Filters);
            context.SorterHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Sorters);
            context.SelectionHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Fields);
            context.QueryHandler.DidNotReceive().Handle(Arg.Any<IQueryable<object>>(), model.Query);
        }

        internal class OozeResolverContext
        {
            public OozeResolver OozeResolver { get; private set; }
            public IOozeFilterHandler FilterHandler { get; } = Substitute.For<IOozeFilterHandler>();
            public IOozeSorterHandler SorterHandler { get; } = Substitute.For<IOozeSorterHandler>();
            public IOozeQueryHandler QueryHandler { get; } = Substitute.For<IOozeQueryHandler>();
            public IOozeSelectionHandler SelectionHandler { get; } = Substitute.For<IOozeSelectionHandler>();
            public OozeConfiguration Configuration { get; } = new OozeConfiguration(new OozeOptions { UseSelections = true })
            {
                EntityConfigurations = new Dictionary<Type, OozeEntityConfiguration>
                {
                    { typeof(object), null }
                }
            };

            public OozeResolverContext()
            {
                OozeResolver = new OozeResolver(SorterHandler, FilterHandler, QueryHandler, SelectionHandler, Configuration);
            }
        }
    }
}