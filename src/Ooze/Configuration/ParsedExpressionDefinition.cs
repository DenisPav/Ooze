using System;
using System.Linq.Expressions;

namespace Ooze.Configuration
{
    internal class ParsedExpressionDefinition
    {
        public string Name { get; set; }
        public Expression Expression { get; set; }
        public Type Type { get; set; }
        public Func<IServiceProvider, IOozeProvider> ProviderFactory { get; set; }
    }
}
