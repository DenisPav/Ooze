using Ooze.Typed.Filters;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.Expressions;

/// <summary>
/// Provides methods for building basic filter expressions
/// </summary>
public static class BasicExpressions
{
    /// <summary>
    /// Creates an equality filter expression
    /// </summary>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterValue">Value to compare against</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> Equal<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue)
        => BasicOperationExpressionFactory(dataExpression, filterValue, Expression.Equal);

    /// <summary>
    /// Creates a not-equal filter expression
    /// </summary>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterValue">Value to compare against</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> NotEqual<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue)
        => BasicOperationExpressionFactory(dataExpression, filterValue, Expression.NotEqual);

    /// <summary>
    /// Creates a greater-than filter expression
    /// </summary>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterValue">Value to compare against</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> GreaterThan<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue)
        => BasicOperationExpressionFactory(dataExpression, filterValue, Expression.GreaterThan);

    /// <summary>
    /// Creates a less-than filter expression
    /// </summary>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterValue">Value to compare against</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> LessThan<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue)
        => BasicOperationExpressionFactory(dataExpression, filterValue, Expression.LessThan);

    /// <summary>
    /// Creates an "In" filter expression
    /// </summary>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterValue">Collection of values to check against</param>
    /// <param name="isNegated">Whether to negate the expression</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> In<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        IEnumerable<TProperty>? filterValue,
        bool isNegated = false)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body, true);
        var genericMethod = CommonMethods.EnumerableContains.MakeGenericMethod(typeof(TProperty));
        var collectionConstantExpression = GetWrappedConstantExpression(filterValue);
        var callExpression = Call(genericMethod, collectionConstantExpression, memberAccessExpression);
        var parameter = ExtractParameterExpression(memberAccessExpression);
        Expression lambdaBody = isNegated
            ? Not(callExpression)
            : callExpression;

        return GetLambdaExpression<TEntity>(lambdaBody, parameter);
    }

    /// <summary>
    /// Creates a range filter expression
    /// </summary>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="rangeFilterValue">Range filter with From and To values</param>
    /// <param name="isNegated">Whether to negate the expression</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> Range<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        RangeFilter<TProperty>? rangeFilterValue,
        bool isNegated = false)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body);
#pragma warning disable CS8602
        var fromConstantExpression = GetWrappedConstantExpression(rangeFilterValue.From);
#pragma warning restore CS8602
        var toConstantExpression = GetWrappedConstantExpression(rangeFilterValue.To);

        var lessThenOrEqualFromExpression = LessThanOrEqual(fromConstantExpression, memberAccessExpression);
        var lessThenOrEqualToExpression = LessThanOrEqual(memberAccessExpression, toConstantExpression);
        var andAlsoExpression = AndAlso(lessThenOrEqualFromExpression, lessThenOrEqualToExpression);
        var parameter = ExtractParameterExpression(memberAccessExpression);
        Expression lambdaBody = isNegated
            ? Not(andAlsoExpression)
            : andAlsoExpression;

        return GetLambdaExpression<TEntity>(lambdaBody, parameter);
    }

    /// <summary>
    /// Creates a string operation filter expression
    /// </summary>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterValue">String value to compare against</param>
    /// <param name="operationMethod">String operation method (e.g., Contains, StartsWith, EndsWith)</param>
    /// <param name="isNegated">Whether to negate the expression</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> StringOperation<TEntity>(
        Expression<Func<TEntity, string>> dataExpression,
        string? filterValue,
        MethodInfo operationMethod,
        bool isNegated = false)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body);
        var stringConstant = GetWrappedConstantExpression(filterValue);
        var callExpression = Call(memberAccessExpression, operationMethod, stringConstant);
        var parameter = ExtractParameterExpression(memberAccessExpression);
        Expression lambdaBody = isNegated
            ? Not(callExpression)
            : callExpression;

        return GetLambdaExpression<TEntity>(lambdaBody, parameter);
    }

    /// <summary>
    /// Extracts a member expression from an expression body
    /// </summary>
    /// <param name="expressionBody">Expression body to extract from</param>
    /// <param name="skipDefaultingToValueProp">Whether to skip defaulting to Value property for nullable types</param>
    /// <returns>Extracted member expression</returns>
    public static MemberExpression GetMemberExpression(
        Expression expressionBody,
        bool skipDefaultingToValueProp = false)
    {
        var memberAccessExpression = expressionBody switch
        {
            UnaryExpression unaryExpression => unaryExpression.Operand as MemberExpression,
            MemberExpression memberExpression => memberExpression,
            _ => throw new Exception("Error while extracting member expression!")
        };

        if (skipDefaultingToValueProp == false && Nullable.GetUnderlyingType(memberAccessExpression!.Type) != null)
            memberAccessExpression = Property(memberAccessExpression, "Value");
        return memberAccessExpression!;
    }

    /// <summary>
    /// Wraps a constant value in an OozeValue wrapper for EF Core parameterization
    /// </summary>
    /// <param name="constant">Constant value to wrap</param>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Expression accessing the wrapped constant</returns>
    public static Expression GetWrappedConstantExpression<TProperty>(TProperty constant)
    {
        var constantType = constant?.GetType() ?? typeof(TProperty);
        var correctType = Nullable.GetUnderlyingType(constantType) ?? constantType;
        var createWrapper = CommonMethods.CreateWrapperObject.MakeGenericMethod(correctType);
        var wrapper = createWrapper?.Invoke(null, [constant!]);

        return Property(Constant(wrapper), nameof(OozeValue<TProperty>.p));
    }

    /// <summary>
    /// Extracts the parameter expression from a member expression
    /// </summary>
    /// <param name="memberExpression">Member expression to extract from</param>
    /// <returns>Parameter expression</returns>
    public static ParameterExpression ExtractParameterExpression(MemberExpression memberExpression)
    {
        var intermediateExpression = memberExpression.Expression;
        while (intermediateExpression is MemberExpression member)
            intermediateExpression = member.Expression;
        return intermediateExpression as ParameterExpression
               ?? throw new Exception("Passed member expression can not be extracted correctly!");
    }

    internal static Expression<Func<TEntity, bool>> GetLambdaExpression<TEntity>(
        Expression body,
        params ParameterExpression[] parameterExpressions)
        => Lambda<Func<TEntity, bool>>(body, parameterExpressions);

    /// <summary>
    /// Used by CommonMethods to create a wrapped object which will be resolved as a parameter in the EF generated SQL query
    /// </summary>
    /// <param name="value">Value to wrap</param>
    /// <typeparam name="TType">Type of value which is being wrapped</typeparam>
    /// <returns>Wrapped value</returns>
    internal static OozeValue<TType> CreateWrapperObject<TType>(TType value)
    {
        return new OozeValue<TType>
        {
            p = value
        };
    }

    private static Expression<Func<TEntity, bool>> BasicOperationExpressionFactory<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        TProperty filterValue,
        Func<Expression, Expression, Expression> operationFactory)
    {
        var memberAccessExpression = GetMemberExpression(dataExpression.Body);
        var result = GetWrappedConstantExpression(filterValue);
        var operationExpression = operationFactory(memberAccessExpression, result);
        var parameter = ExtractParameterExpression(memberAccessExpression);

        return GetLambdaExpression<TEntity>(operationExpression, parameter);
    }
}