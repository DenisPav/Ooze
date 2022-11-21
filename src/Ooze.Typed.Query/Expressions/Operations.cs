using System.Linq.Expressions;
using Ooze.Typed.Query.Tokenization;

namespace Ooze.Typed.Query.Expressions;

internal static class Operations
{
    public static readonly IReadOnlyDictionary<string, Func<Expression, Expression, Expression>>
        OperatorExpressionFactories = new Dictionary<string, Func<Expression, Expression, Expression>>()
        {
            { QueryTokenizer.GreaterThan, Expression.GreaterThan },
            { QueryTokenizer.GreaterThanOrEqual, Expression.GreaterThanOrEqual },
            { QueryTokenizer.LessThan, Expression.LessThan },
            { QueryTokenizer.LessThanOrEqual, Expression.LessThanOrEqual },
            { QueryTokenizer.EqualTo, Expression.Equal },
            { QueryTokenizer.NotEqualTo, Expression.NotEqual },
        };
}