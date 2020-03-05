using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ooze.Configuration
{
    public class Expressions
    {
        public ParameterExpression Param { get; internal set; }
        public IEnumerable<(string, LambdaExpression, Type)> LambdaExpressions { get; internal set; }
    }
}
