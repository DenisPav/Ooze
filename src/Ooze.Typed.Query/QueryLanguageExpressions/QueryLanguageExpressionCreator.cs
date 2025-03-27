using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Ooze.Typed.Expressions;
using Ooze.Typed.Query.Exceptions;
using Ooze.Typed.Query.Filters;
using Ooze.Typed.Query.Tokenization;
using Superpower.Model;

namespace Ooze.Typed.Query.QueryLanguageExpressions;

internal static class QueryLanguageExpressionCreator
{
    private const string ParameterName = "x";
    private const string Where = nameof(Where);

    public static ExpressionResult Create<TEntity>(
        ICollection<QueryLanguageFilterDefinition<TEntity>> filterDefinitions,
        Expression queryExpression,
        ICollection<Token<QueryLanguageToken>> queryDefinitionTokens)
    {
        var parameterExpression = Expression.Parameter(
            typeof(TEntity),
            ParameterName
        );

        var tokenStack = new Stack<Token<QueryLanguageToken>>(queryDefinitionTokens.Reverse());
        var bracketStack = new Stack<Token<QueryLanguageToken>>();

        try
        {
            var createdExpr = CreateExpression(filterDefinitions, parameterExpression, tokenStack, bracketStack);
            if (bracketStack.Any())
                throw new ExpressionCreatorException("no matching ending bracket");

            var finalExpression = Expression.Lambda<Func<TEntity, bool>>(createdExpr, parameterExpression);
            var quoteExpr = Expression.Quote(finalExpression);
            var callExpr = Expression.Call(typeof(Queryable), Where, [typeof(TEntity)], queryExpression,
                quoteExpr);

            return new ExpressionResult(callExpr, null);
        }
        catch (Exception e)
        {
            return new ExpressionResult(null, new ExpressionCreatorException(e.Message));
        }
    }

    private static Expression? CreateExpression<TEntity>(
        ICollection<QueryLanguageFilterDefinition<TEntity>> filterDefinitions,
        ParameterExpression parameterExpression,
        Stack<Token<QueryLanguageToken>> tokenStack,
        Stack<Token<QueryLanguageToken>> bracketStack)
    {
        var propertyExpressions = new List<Expression>();
        var logicalOperationExpressions = new List<Func<Expression, Expression, Expression>>();

        while (tokenStack.Any())
        {
            var token = tokenStack.Peek();
            switch (token.Kind)
            {
                case QueryLanguageToken.Property:
                    CreateMemberExpression(filterDefinitions, parameterExpression, tokenStack,
                        propertyExpressions);
                    break;
                case QueryLanguageToken.LogicalOperation:
                    tokenStack.Pop();
                    var isAnd = token.Span.ToStringValue().Contains("&&");
                    logicalOperationExpressions.Add(isAnd ? Expression.AndAlso : Expression.OrElse);
                    break;
                case QueryLanguageToken.BracketLeft:
                    tokenStack.Pop();
                    bracketStack.Push(token);
                    var subExpr = CreateExpression(filterDefinitions, parameterExpression, tokenStack, bracketStack);
                    propertyExpressions.Add(subExpr);
                    break;
                case QueryLanguageToken.BracketRight:
                    tokenStack.Pop();
                    if (bracketStack.Any() == false)
                        throw new ExpressionCreatorException("no matching start bracket");

                    bracketStack.Pop();
                    return CreateLogicalExpression(logicalOperationExpressions, propertyExpressions);
                case QueryLanguageToken.Operation:
                case QueryLanguageToken.Value:
                    throw new ExpressionCreatorException("wrong order of tokens found in query");
            }
        }

        return CreateLogicalExpression(logicalOperationExpressions, propertyExpressions);
    }

    private static Expression? CreateLogicalExpression(
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
    /// <param name="filterDefinitions">List of query language filter definitions</param>
    /// <param name="parameterExpression">Root parameter used for building expression</param>
    /// <param name="tokens">Current stack of tokens</param>
    /// <param name="propertyExpressions">List of member current member expressions</param>
    /// <typeparam name="TEntity">Type of Queryable instance</typeparam>
    private static void CreateMemberExpression<TEntity>(
        IEnumerable<QueryLanguageFilterDefinition<TEntity>> filterDefinitions,
        ParameterExpression parameterExpression,
        Stack<Token<QueryLanguageToken>> tokens,
        ICollection<Expression> propertyExpressions)
    {
        var token = tokens.Pop();
        var queryPropertyName = token.ToStringValue();
        var filterDefinition = filterDefinitions.Single(definition =>
                string.Compare(queryPropertyName, definition.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
        
        token = tokens.Pop();
        var operationExpressionFactory = Operations.OperatorExpressionFactories[token.ToStringValue()];
        token = tokens.Pop();

        var value = token.ToStringValue();
        var clearValue = value.Replace(QueryLanguageTokenizer.ValueTick, string.Empty);
        //TODO: maybe move this to definition so it fails there if this is not a property
        var propertyInfo = (filterDefinition.MemberExpression.Member as PropertyInfo).PropertyType;
        var convertedValue = TypeDescriptor.GetConverter(propertyInfo).ConvertFrom(clearValue);
        var valueExpression = BasicExpressions.GetWrappedConstantExpression(convertedValue);

        var replaced = new ParameterReplacerVisitor(parameterExpression).Visit(filterDefinition.MemberExpression);
        var finalOperationExpression = operationExpressionFactory(replaced, valueExpression);
        propertyExpressions.Add(finalOperationExpression);
    }
}

internal record ExpressionResult(Expression? Expression, Exception? Error);