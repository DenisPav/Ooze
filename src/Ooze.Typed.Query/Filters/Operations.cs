using System.Linq.Expressions;

namespace Ooze.Typed.Query.Filters;

internal static class Operations
{
    private const string GreaterThan = ">";
    // private const string GreaterThanOrEqual = ">@";
    private const string LessThan = "<";
    // private const string LessThanOrEqual = "<@";
    private const string EqualTo = "==";
    private const string NotEqualTo = "!=";

    public static readonly IReadOnlyDictionary<string, Func<Expression, Expression, Expression>>
        OperatorExpressionFactories = new Dictionary<string, Func<Expression, Expression, Expression>>()
        {
            { GreaterThan, Expression.GreaterThan },
            // { GreaterThanOrEqual, Expression.GreaterThanOrEqual },
            { LessThan, Expression.LessThan },
            // { LessThanOrEqual, Expression.LessThanOrEqual },
            { EqualTo, Expression.Equal },
            { NotEqualTo, Expression.NotEqual },
        };
}