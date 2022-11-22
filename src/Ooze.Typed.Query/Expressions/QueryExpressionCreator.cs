using System.Linq.Expressions;
using Ooze.Typed.Query.Tokenization;
using Superpower.Model;

namespace Ooze.Typed.Query.Expressions;

internal static class QueryExpressionCreator
{
    private const string ParameterName = "x";
    private const string Where = nameof(Where);

    public static ExpressionResult Create<TEntity>(
        Expression queryExpression,
        IEnumerable<Token<QueryToken>> queryDefinitionTokens)
    {
        var parameterExpression = Expression.Parameter(
            typeof(TEntity),
            ParameterName
        );
        
        var tokenStack = new Stack<Token<QueryToken>>(queryDefinitionTokens.Reverse());
        var bracketStack = new Stack<Token<QueryToken>>();

        try
        {
            var createdExpr = CreateExpression<TEntity>(parameterExpression, tokenStack, bracketStack);
            var finalExpression = Expression.Lambda<Func<TEntity, bool>>(createdExpr, parameterExpression);
            var quoteExpr = Expression.Quote(finalExpression);
            var callExpr = Expression.Call(typeof(Queryable), Where, new[] { typeof(TEntity) }, queryExpression,
                quoteExpr);

            return new ExpressionResult(callExpr, null);
        }
        catch (Exception e)
        {
            return new ExpressionResult(null, e);
        }
    }

    private static Expression CreateExpression<TEntity>(
        ParameterExpression parameterExpression,
        Stack<Token<QueryToken>> tokenStack,
        Stack<Token<QueryToken>> bracketStack)
    {
        var propertyExpressions = new List<Expression>();
        var logicalOperationExpressions = new List<Func<Expression, Expression, Expression>>();

        while (tokenStack.Any())
        {
            var token = tokenStack.Peek();
            switch (token.Kind)
            {
                case QueryToken.Property:
                    CreateMemberExpression<TEntity>(parameterExpression, tokenStack, propertyExpressions);
                    break;
                case QueryToken.LogicalOperation:
                    tokenStack.Pop();
                    logicalOperationExpressions.Add(Expression.AndAlso);
                    break;
                case QueryToken.BracketLeft:
                    tokenStack.Pop();
                    bracketStack.Push(token);
                    var subExpr = CreateExpression<TEntity>(parameterExpression, tokenStack, bracketStack);
                    propertyExpressions.Add(subExpr);
                    break;

                case QueryToken.BracketRight:
                    tokenStack.Pop();
                    if (bracketStack.Any() == false)
                        throw new Exception("no matching start bracket");

                    bracketStack.Pop();
                    return CreateLogicalExpression(logicalOperationExpressions, propertyExpressions);
            }
        }

        return CreateLogicalExpression(logicalOperationExpressions, propertyExpressions);
    }

    private static Expression CreateLogicalExpression(
        IList<Func<Expression, Expression, Expression>> logicalOperationExpressions,
        IEnumerable<Expression> propertyExpressions)
    {
        if (logicalOperationExpressions.Any() == false)
            return propertyExpressions.FirstOrDefault();

        var skip = 0;
        var expression = logicalOperationExpressions.Aggregate((Expression)null, (agg, current) =>
        {
            if (agg == null)
            {
                var args = propertyExpressions.Skip(skip).Take(2).ToArray();
                skip += 2;
                return current(args[0], args[1]);
            }
            else
            {
                var args = propertyExpressions.Skip(skip).Take(1).ToArray();
                skip += 1;
                return current(agg, args[0]);
            }
        });
        return expression;
    }

    /// <summary>
    /// Take next 3 tokens and create member expression from them.
    /// </summary>
    /// <param name="parameterExpression">Root parameter used for building expression</param>
    /// <param name="tokens">Current stack of tokens</param>
    /// <param name="propertyExpressions">List of member current member expressions</param>
    /// <typeparam name="TEntity">Type of Queryable instance</typeparam>
    private static void CreateMemberExpression<TEntity>(
        ParameterExpression parameterExpression,
        Stack<Token<QueryToken>> tokens,
        ICollection<Expression> propertyExpressions)
    {
        var token = tokens.Pop();
        var property = typeof(TEntity).GetProperty(token.ToStringValue());
        var currentMemberAccess = Expression.MakeMemberAccess(parameterExpression, property);
        token = tokens.Pop();
        var operationExpressionFactory = Operations.OperatorExpressionFactories[token.ToStringValue()];
        token = tokens.Pop();

        var value = token.ToStringValue();
        var clearValue = value.Replace(QueryTokenizer.ValueTick, string.Empty);
        var convertedValue = Convert.ChangeType(clearValue, property.PropertyType);
        var valueExpression = Expression.Constant(convertedValue);

        var finalOperationExpression = operationExpressionFactory(currentMemberAccess, valueExpression);
        propertyExpressions.Add(finalOperationExpression);
    }
}

internal record class ExpressionResult(Expression Expression, Exception Error);