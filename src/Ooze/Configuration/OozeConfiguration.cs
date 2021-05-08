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
        public readonly IReadOnlyDictionary<string, (Operation operation, Func<Type, bool> validator)> OperationsMap;
        public readonly IReadOnlyDictionary<string, Operation> LogicalOperationMap = new Dictionary<string, Operation>
        {
            { "AND", AndAlso },
            { "OR", OrElse}
        };
        public readonly bool UseSelections = false;
        public readonly bool UsePaging = false;
        public readonly int DefaultPageSize = 20;

        public OozeConfiguration(
            OozeOptions options)
        {
            _ = options ?? throw new Exception("Ooze options not registered to container");

            OperationsMap = new Dictionary<string, (Operation, Func<Type, bool> validator)>
            {
                { options.Operations.Equal, (Equal, DefaultTypeValidator) },
                { options.Operations.NotEqual, (NotEqual, DefaultTypeValidator) },
                { options.Operations.GreaterThanOrEqual, (GreaterThanOrEqual, DefaultTypeValidator) },
                { options.Operations.LessThanOrEqual, (LessThanOrEqual, DefaultTypeValidator) },
                { options.Operations.StartsWith, (Expressions.StartsWith, type => type == typeof(string)) },
                { options.Operations.EndsWith, (Expressions.EndsWith, type => type == typeof(string)) },
                { options.Operations.GreaterThan, (GreaterThan, DefaultTypeValidator) },
                { options.Operations.LessThan, (LessThan, DefaultTypeValidator) },
                { options.Operations.Contains, (Expressions.Contains, type => type == typeof(string)) },
            };
            UseSelections = options.UseSelections;
            UsePaging = options.Paging.UsePaging;
            DefaultPageSize = options.Paging.DefaultPageSize;
        }

        public IDictionary<Type, OozeEntityConfiguration> EntityConfigurations { get; set; }
        public List<Func<IServiceProvider, IOozeProvider>> ProviderFactories
            => EntityConfigurations.Values
                .SelectMany(config => config.ProviderFactories)
                .ToList();

        static bool DefaultTypeValidator(Type type) => true;
    }
}
