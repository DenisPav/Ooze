using System.Linq.Expressions;
using System.Reflection;
using Ooze.Typed.Query.Tokenization;

namespace Ooze.Typed.Query.QueryLanguageExpressions;

internal static class Operations
{
    private static readonly Type StringType = typeof(string);
    private static readonly MethodInfo StartsWith = StringType.GetMethod("StartsWith", [StringType])!;
    private static readonly MethodInfo EndsWith = StringType.GetMethod("EndsWith", [StringType])!;
    private static readonly MethodInfo Contains = StringType.GetMethod("Contains", [StringType])!;

    public static readonly IReadOnlyDictionary<string, Func<Expression, Expression, Expression>>
        OperatorExpressionFactories = new Dictionary<string, Func<Expression, Expression, Expression>>
        {
            { QueryLanguageTokenizer.GreaterThan, Expression.GreaterThan },
            { QueryLanguageTokenizer.GreaterThanOrEqual, Expression.GreaterThanOrEqual },
            { QueryLanguageTokenizer.LessThan, Expression.LessThan },
            { QueryLanguageTokenizer.LessThanOrEqual, Expression.LessThanOrEqual },
            { QueryLanguageTokenizer.EqualTo, Expression.Equal },
            { QueryLanguageTokenizer.NotEqualTo, Expression.NotEqual },
            { QueryLanguageTokenizer.StartsWith, CreateStartsWithExpression },
            { QueryLanguageTokenizer.EndsWith, CreateEndsWithExpression },
            { QueryLanguageTokenizer.Contains, CreateContainsExpression },
        };

    private static Expression CreateStartsWithExpression(
        Expression left,
        Expression right)
        => Expression.Call(left, StartsWith, right);

    private static Expression CreateEndsWithExpression(
        Expression left,
        Expression right)
        => Expression.Call(left, EndsWith, right);

    private static Expression CreateContainsExpression(
        Expression left,
        Expression right)
        => Expression.Call(left, Contains, right);
}