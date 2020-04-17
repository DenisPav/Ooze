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
        public readonly IReadOnlyDictionary<string, Operation> OperationsMap;
        public readonly IReadOnlyDictionary<string, Operation> LogicalOperationMap = new Dictionary<string, Operation>
        {
            { "AND", AndAlso },
            { "OR", OrElse}
        };
        public readonly bool UseSelections = false;

        public OozeConfiguration(
            OozeOptions options)
        {
            _ = options ?? throw new Exception("Ooze options not registered to container");

            OperationsMap = new Dictionary<string, Operation>
            {
                { options.Operations.Equal, Equal },
                { options.Operations.NotEqual, NotEqual },
                { options.Operations.GreaterThanOrEqual, GreaterThanOrEqual },
                { options.Operations.LessThanOrEqual, LessThanOrEqual },
                { options.Operations.StartsWith, Expressions.StartsWith },
                { options.Operations.EndsWith, Expressions.EndsWith },
                { options.Operations.GreaterThan, GreaterThan },
                { options.Operations.LessThan, LessThan },
                { options.Operations.Contains, Expressions.Contains },
            };
            UseSelections = options.UseSelections;
        }

        public IDictionary<Type, OozeEntityConfiguration> EntityConfigurations { get; set; }
        public List<Func<IServiceProvider, IOozeProvider>> ProviderFactories
            => EntityConfigurations.Values
                .SelectMany(config => config.ProviderFactories)
                .ToList();
    }
}
