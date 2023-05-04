using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ooze.Configuration
{
    internal class OozeEntityConfiguration
    {
        public ParameterExpression Param { get; set; }
        public IEnumerable<ParsedExpressionDefinition> Sorters { get; set; }
        public IEnumerable<ParsedExpressionDefinition> Filters { get; set; }
        public IEnumerable<Func<IServiceProvider, IOozeProvider>> ProviderFactories
            => Sorters.Select(sorter => sorter.ProviderFactory)
                .Concat(Filters.Select(filter => filter.ProviderFactory));
    }
}
