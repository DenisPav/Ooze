using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.Filters;

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
        var constantExpression = Constant(filterValue);
        var opreationExpression = operationFactory(memberAccessExpression, constantExpression);
        var parameter = memberAccessExpression.Expression as ParameterExpression;

        return GetLambdaExpression<TEntity>(opreationExpression, parameter);
    }

    internal static Expression<Func<TEntity, bool>> In<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        IEnumerable<TProperty> filterValue)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body);
        var genericMethod = Common.EnumerableContains.MakeGenericMethod(typeof(TProperty));
        var collectionConstantExpression = Constant(filterValue);
        var callExpression = Call(genericMethod, collectionConstantExpression, memberAccessExpression);
        var parameter = memberAccessExpression.Expression as ParameterExpression;

        return GetLambdaExpression<TEntity>(callExpression, parameter);
    }

    internal static Expression<Func<TEntity, bool>> Range<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        RangeFilter<TProperty> rangeFilterValue)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body);
        var fromConstantExpression = Constant(rangeFilterValue.From);
        var toConstantExpression = Constant(rangeFilterValue.To);

        var lessThenOrEqualFromExpression = LessThanOrEqual(fromConstantExpression, memberAccessExpression);
        var lessThenOrEqualToExpression = LessThanOrEqual(memberAccessExpression, toConstantExpression);
        var andAlsoExpression = AndAlso(lessThenOrEqualFromExpression, lessThenOrEqualToExpression);
        var parameter = memberAccessExpression.Expression as ParameterExpression;

        return GetLambdaExpression<TEntity>(andAlsoExpression, parameter);
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
}
