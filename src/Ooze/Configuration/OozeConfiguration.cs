using Ooze.Configuration.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Configuration
{
    internal delegate Expression Operation(Expression left, Expression right);

    internal class OozeConfiguration
    {
        readonly OozeOptions _options;
        public readonly IReadOnlyDictionary<string, Operation> OperationsMap;
        public readonly IReadOnlyDictionary<string, Operation> LogicalOperationMap = new Dictionary<string, Operation>
        {
            { "AND", AndAlso },
            { "OR", OrElse}
        };

        public OozeConfiguration(
            OozeOptions options)
        {
            _options = options ?? throw new Exception("Ooze options not registered to container");

            OperationsMap = new Dictionary<string, Operation>
            {
                { _options.Operations.Equal, Equal },
                { _options.Operations.NotEqual, NotEqual },
                { _options.Operations.GreaterThanOrEqual, GreaterThanOrEqual },
                { _options.Operations.LessThanOrEqual, LessThanOrEqual },
                { _options.Operations.StartsWith, Expressions.StartsWith },
                { _options.Operations.EndsWith, Expressions.EndsWith },
                { _options.Operations.GreaterThan, GreaterThan },
                { _options.Operations.LessThan, LessThan },
                { _options.Operations.Contains, Expressions.Contains },
            };
        }

        public IDictionary<Type, OozeEntityConfiguration> EntityConfigurations { get; set; }
        public List<Func<IServiceProvider, IOozeProvider>> ProviderFactories
            => EntityConfigurations.Values
                .SelectMany(config => config.ProviderFactories)
                .ToList();
    }
}
