using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Configuration
{
    public class OozeConfiguration
    {
        public delegate Expression Operation(Expression left, Expression right);
        public readonly IReadOnlyDictionary<string, Operation> OperationsMap = new Dictionary<string, Operation>
        {
            //some of these arent available on strings, for example greater than
            { "==", Equal },
            { "!=", NotEqual },
            { ">=", GreaterThanOrEqual },
            { "<=", LessThanOrEqual },
            { "@=", Expressions.StartsWith },
            { "=@", Expressions.EndsWith },
            { ">", GreaterThan },
            { "<", LessThan },
            { "@", Expressions.Contains },
        };

        public IDictionary<Type, OozeEntityConfiguration> EntityConfigurations { get; set; }
    }
}
