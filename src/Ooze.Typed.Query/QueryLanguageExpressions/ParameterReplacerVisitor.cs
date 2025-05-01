using System.Linq.Expressions;

namespace Ooze.Typed.Query.QueryLanguageExpressions;

internal class ParameterReplacerVisitor(ParameterExpression newParameterExpression) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node) 
        => newParameterExpression;
}