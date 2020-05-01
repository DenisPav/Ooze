using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Query;
using Ooze.Selections;
using System;
using System.Collections;
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
        const string Select = nameof(Select);
        const string ToList = nameof(ToList);
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
            Expression paramExpr,
            PropertyInfo targetProperty,
            LambdaExpression expression)
            => Call(
                typeof(Enumerable),
                Select,
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
                ToList,
                new[] {
                    propertyType
                },
                selectExpression
                );

        public static MemberInitExpression MemberInitExpr(
            Type propertyType,
            IEnumerable<MemberAssignment> memberAssignments)
            => MemberInit(
                New(
                    propertyType.GetConstructors().First()
                    ),
                memberAssignments
                );

        public static LambdaExpression LambdaExpr(
            Type propertyType,
            ParameterExpression parameterExpression,
            IEnumerable<MemberAssignment> memberAssignments)
            => Lambda(
                MemberInitExpr(propertyType, memberAssignments),
                parameterExpression
                );

        public static IEnumerable<MemberAssignment> CreateAssignments(
            Expression expression,
            Type type,
            IEnumerable<FieldDefinition> fieldDefinitions)
        {
            var typeProperties = type.GetProperties();
            foreach (var fieldDefinition in fieldDefinitions)
            {
                if (!fieldDefinition.Children.Any())
                {
                    var targetProp = typeProperties.FirstOrDefault(prop => prop.Name.Equals(fieldDefinition.Property, StringComparison.InvariantCultureIgnoreCase));
                    if (targetProp is { })
                    {
                        yield return Bind(targetProp, MakeMemberAccess(expression, targetProp));
                    }
                }
                else
                {
                    var targetProp = typeProperties.FirstOrDefault(prop => prop.Name.Equals(fieldDefinition.Property, StringComparison.InvariantCultureIgnoreCase));
                    if (targetProp is { } && typeof(IEnumerable).IsAssignableFrom(targetProp.PropertyType))
                    {
                        var propertyType = targetProp.PropertyType.GetGenericArguments().First();

                        yield return NestedSelectExpr(expression, fieldDefinition, targetProp, propertyType);
                    }
                    else if (targetProp is { } && targetProp.PropertyType.IsClass)
                    {
                        yield return NestedClassInitExpr(expression, fieldDefinition, targetProp);
                    }
                }
            }
        }

        static MemberAssignment NestedSelectExpr(
            Expression expression,
            FieldDefinition fieldDefinition,
            PropertyInfo targetProp,
            Type propertyType)
        {
            var newRootParamExpr = Parameter(propertyType, targetProp.Name.ToLower());
            var nestedAssignments = CreateAssignments(newRootParamExpr, propertyType, fieldDefinition.Children);
            var lambda = LambdaExpr(propertyType, newRootParamExpr, nestedAssignments);
            var selectCallExpr = SelectExpr(propertyType, expression, targetProp, lambda);
            var toListCallExpr = ToListExpr(propertyType, selectCallExpr);

            return Bind(targetProp, toListCallExpr);
        }

        static MemberAssignment NestedClassInitExpr(
            Expression expression,
            FieldDefinition fieldDefinition,
            PropertyInfo targetProp)
        {
            var propertyType = targetProp.PropertyType;
            var parentObjectExpression = MakeMemberAccess(expression, targetProp);
            var nestedAssignments = CreateAssignments(parentObjectExpression, propertyType, fieldDefinition.Children);
            var memberInit = MemberInitExpr(propertyType, nestedAssignments);

            return Bind(targetProp, memberInit);
        }
    }
}
