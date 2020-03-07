﻿using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Configuration
{
    public class OozeConfiguration
    {
        public delegate BinaryExpression Operation(Expression left, Expression right);
        public readonly IReadOnlyDictionary<string, Operation> OperationsMap = new Dictionary<string, Operation>
        {
            { "==", Equal },
            { "!=", NotEqual },
            { ">=", GreaterThanOrEqual },
            { "<=", LessThanOrEqual },
            { ">", GreaterThan },
            { "<", LessThan },
        };

        public IEnumerable<OozeEntityConfiguration> EntityConfigurations { get; set; }
    }
}
