using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ooze.Configuration
{
    public class ParsedExpressionDefinition
    {
        public string Name { get; internal set; }
        public Expression Expression { get; internal set; }
        public Type Type { get; internal set; }
    }
}
