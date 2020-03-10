using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ooze.Configuration
{
    public class OozeEntityConfiguration
    {
        public ParameterExpression Param { get; internal set; }
        public IEnumerable<ParsedExpressionDefinition> Sorters { get; internal set; }
        public IEnumerable<ParsedExpressionDefinition> Filters { get; internal set; }
    }
}
