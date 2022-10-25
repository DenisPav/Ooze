﻿using Ooze.Typed.Filters;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.Expressions;

internal static class BasicExpressions
{
    internal static Expression<Func<TEntity, bool>> Equal<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue)
        => BasicOperationExpressionFactory(dataExpression, filterValue, Expression.Equal);

    internal static Expression<Func<TEntity, bool>> NotEqual<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue)
        => BasicOperationExpressionFactory(dataExpression, filterValue, Expression.NotEqual);

    internal static Expression<Func<TEntity, bool>> GreaterThan<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue)
        => BasicOperationExpressionFactory(dataExpression, filterValue, Expression.GreaterThan);

    internal static Expression<Func<TEntity, bool>> LessThan<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue)
        => BasicOperationExpressionFactory(dataExpression, filterValue, Expression.LessThan);

    internal static Expression<Func<TEntity, bool>> BasicOperationExpressionFactory<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue,
        Func<Expression, Expression, Expression> operationFactory)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body);
        var result = GetWrappedConstantExpression(filterValue);
        var opreationExpression = operationFactory(memberAccessExpression, result);
        var parameter = memberAccessExpression.Expression as ParameterExpression;

        return GetLambdaExpression<TEntity>(opreationExpression, parameter);
    }

    internal static Expression<Func<TEntity, bool>> In<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        IEnumerable<TProperty> filterValue,
        bool isNegated = false)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body);
        var genericMethod = CommonMethods.EnumerableContains.MakeGenericMethod(typeof(TProperty));
        var collectionConstantExpression = GetWrappedConstantExpression(filterValue);
        var callExpression = Call(genericMethod, collectionConstantExpression, memberAccessExpression);
        var parameter = memberAccessExpression.Expression as ParameterExpression;
        Expression lambaBody = isNegated
            ? Not(callExpression)
            : callExpression;

        return GetLambdaExpression<TEntity>(lambaBody, parameter);
    }

    internal static Expression<Func<TEntity, bool>> Range<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        RangeFilter<TProperty> rangeFilterValue,
        bool isNegated = false)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body);
        var fromConstantExpression = GetWrappedConstantExpression(rangeFilterValue.From);
        var toConstantExpression = GetWrappedConstantExpression(rangeFilterValue.To);

        var lessThenOrEqualFromExpression = LessThanOrEqual(fromConstantExpression, memberAccessExpression);
        var lessThenOrEqualToExpression = LessThanOrEqual(memberAccessExpression, toConstantExpression);
        var andAlsoExpression = AndAlso(lessThenOrEqualFromExpression, lessThenOrEqualToExpression);
        var parameter = memberAccessExpression.Expression as ParameterExpression;
        Expression lambaBody = isNegated
            ? Not(andAlsoExpression)
            : andAlsoExpression;

        return GetLambdaExpression<TEntity>(lambaBody, parameter);
    }

    internal static Expression<Func<TEntity, bool>> StringOperation<TEntity>(
        Expression<Func<TEntity, string>> dataExpression,
        string filterValue,
        MethodInfo operationMethod,
        bool isNegated = false)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body);
        var stringConstant = GetWrappedConstantExpression(filterValue);
        var callExpression = Call(memberAccessExpression, operationMethod, stringConstant);
        var parameter = memberAccessExpression.Expression as ParameterExpression;
        Expression lambaBody = isNegated
            ? Not(callExpression)
            : callExpression;

        return GetLambdaExpression<TEntity>(lambaBody, parameter);
    }

    internal static MemberExpression GetMemberExpression(Expression expressionBody)
    {
        var memberAccessExpression = expressionBody switch
        {
            UnaryExpression unaryExpression => unaryExpression.Operand as MemberExpression,
            MemberExpression memberExpression => memberExpression,
            _ => throw new Exception("not handled yet")
        };

        return memberAccessExpression;
    }

    internal static Expression<Func<TEntity, bool>> GetLambdaExpression<TEntity>(
        Expression body,
        params ParameterExpression[] parameterExpressions)
        => Lambda<Func<TEntity, bool>>(body, parameterExpressions);

    internal static Expression GetWrappedConstantExpression<TProperty>(TProperty constant)
    {
        var constantType = typeof(TProperty);
        var correctType = Nullable.GetUnderlyingType(constantType) ?? constantType;
        var createWrapper = CommonMethods.CreateWrapperObject.MakeGenericMethod(correctType);
        var wrapper = createWrapper?.Invoke(null, new object[] { constant });

        return Property(Constant(wrapper), nameof(OozeValue<TProperty>.p));
    }
    
    internal static string GetExpressionName<TEntity, TTarget>(Expression<Func<TEntity, TTarget>> expression)
    {
        if (!(expression.Body is MemberExpression memberExpression))
        {
            throw new Exception("Passed expression not correctly defined");
        }

        var memberName = memberExpression.Member.Name;
        return memberName;
    }

    internal static OozeValue<TType> CreateWrapperObject<TType>(TType value)
    {
        return new OozeValue<TType>
        {
            p = value
        };
    }
}
