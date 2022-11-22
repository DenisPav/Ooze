using System.Linq.Expressions;
using System.Reflection;
using Ooze.Typed.Query.Tokenization;

namespace Ooze.Typed.Query.Expressions;

internal static class Operations
{
    private static readonly MethodInfo StartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
    private static readonly MethodInfo EndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
    private static readonly MethodInfo Contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });

    public static readonly IReadOnlyDictionary<string, Func<Expression, Expression, Expression>>
        OperatorExpressionFactories = new Dictionary<string, Func<Expression, Expression, Expression>>()
        {
            { QueryTokenizer.GreaterThan, Expression.GreaterThan },
            { QueryTokenizer.GreaterThanOrEqual, Expression.GreaterThanOrEqual },
            { QueryTokenizer.LessThan, Expression.LessThan },
            { QueryTokenizer.LessThanOrEqual, Expression.LessThanOrEqual },
            { QueryTokenizer.EqualTo, Expression.Equal },
            { QueryTokenizer.NotEqualTo, Expression.NotEqual },
            { QueryTokenizer.StartsWith, CreateStartsWithExpression },
            { QueryTokenizer.EndsWith, CreateEndsWithExpression },
            { QueryTokenizer.Contains, CreateContainsExpression },
        };

    private static Expression CreateStartsWithExpression(Expression left, Expression right) =>
        Expression.Call(left, StartsWith, right);
    private static Expression CreateEndsWithExpression(Expression left, Expression right) =>
        Expression.Call(left, EndsWith, right);
    private static Expression CreateContainsExpression(Expression left, Expression right) =>
        Expression.Call(left, Contains, right);
}