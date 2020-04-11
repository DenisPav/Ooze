using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Ooze.Expressions
{
    internal static class OozeExpressionCreator
    {
        const string Where = nameof(Where);
        const string OrderBy = nameof(OrderBy);
        const string ThenBy = nameof(ThenBy);
        const string ThenByDescending = nameof(ThenByDescending);
        const string OrderByDescending = nameof(OrderByDescending);
        static readonly Type _queryableType = typeof(Queryable);
        static readonly Type _stringType = typeof(string);

        public static MethodCallExpression FilterExpression<TEntity>(
            ParameterExpression parameterExpression,
            FilterParserResult parsedFilter,
            ParsedExpressionDefinition def,
            Expression expr,
            Operation operationExprFactory)
        {
            var typings = new[] { typeof(TEntity) };

            var typeConverter = TypeDescriptor.GetConverter(def.Type);
            var value = typeConverter.CanConvertFrom(_stringType)
                ? typeConverter.ConvertFrom(parsedFilter.Value)
                : System.Convert.ChangeType(parsedFilter.Value, def.Type);

            var constValueExpr = Constant(value);
            var operationExpr = operationExprFactory(def.Expression, constValueExpr);
            var lambda = Lambda(operationExpr, parameterExpression);
            var quoteExpr = Quote(lambda);

            var callExpr = Call(_queryableType, Where, typings, expr, quoteExpr);
            return callExpr;
        }

        public static MethodCallExpression SortExpression<TEntity>(
            Expression queryExpression,
            Expression sorterExpression,
            Type sorterType,
            bool ascending,
            bool isFirst = true)
        {
            var typings = new[] { typeof(TEntity), sorterType };
            var quoteExpr = Quote(sorterExpression);

            var method = isFirst
                ? ascending ? OrderBy : OrderByDescending
                : ascending ? ThenBy : ThenByDescending;

            return Call(_queryableType, method, typings, queryExpression, quoteExpr);
        }

        public static MethodCallExpression QueryExpression<TEntity>(
            OozeEntityConfiguration configuration,
            IEnumerable<QueryFilterOperation> mappedQueryParts,
            Expression expr)
        {
            var typings = new[] { typeof(TEntity) };
            var exprs = mappedQueryParts.Select(OperationExpression)
                .ToList();

            Expression endingExpr = null;
            for (int i = 0; i < exprs.Count; i++)
            {
                var (currentExpr, _) = exprs[i];
                if (i == 0)
                {
                    endingExpr = currentExpr;
                }
                else
                {
                    var (_, before) = exprs[i - 1];
                    endingExpr = before.LogicalOperationFactory(endingExpr, currentExpr);
                }
            }

            var lambda = Lambda(endingExpr, configuration.Param);
            var quoteExpr = Quote(lambda);
            var callExpr = Call(_queryableType, Where, typings, expr, quoteExpr);
            return callExpr;
        }

        static (Expression operationExpr, QueryFilterOperation mappedPart) OperationExpression(
            QueryFilterOperation mappedPart)
        {
            var typeConverter = TypeDescriptor.GetConverter(mappedPart.Filter.Type);
            var value = typeConverter.CanConvertFrom(_stringType)
                ? typeConverter.ConvertFrom(mappedPart.QueryPart.Value)
                : System.Convert.ChangeType(mappedPart.QueryPart.Value, mappedPart.Filter.Type);

            var constValueExpr = Constant(value);
            var operationExpr = mappedPart.OperationFactory(mappedPart.Filter.Expression, constValueExpr);

            return (operationExpr, mappedPart);
        }

        public static MethodCallExpression SelectExpr(
            Type propertyType,
            ParameterExpression paramExpr,
            PropertyInfo targetProperty,
            LambdaExpression expression)
            => Call(
                typeof(Enumerable),
                "Select",
                new[] {
                    propertyType,
                    propertyType
                },
                MakeMemberAccess(paramExpr, targetProperty),
                expression
                );

        public static MethodCallExpression ToListExpr(
            Type propertyType,
            MethodCallExpression selectExpression)
            => Call(
                typeof(Enumerable),
                "ToList",
                new[] {
                    propertyType
                },
                selectExpression
                );

        public static LambdaExpression LambdaExpr(
            Type propertyType,
            ParameterExpression parameterExpression,
            IEnumerable<MemberAssignment> memberAssignments)
            => Lambda(
                MemberInit(
                    New(
                        propertyType.GetConstructors().First()
                        ),
                    memberAssignments
                    ),
                parameterExpression
                );
    }
}
