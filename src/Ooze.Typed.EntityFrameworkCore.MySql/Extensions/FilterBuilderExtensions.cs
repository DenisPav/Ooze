using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Expressions;
using Ooze.Typed.Filters;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.MySql.Extensions;

/// <summary>
/// MySql/MariaDb extensions for FilterBuilder
/// </summary>
public static class FilterBuilderExtensions
{
    private static readonly Type DbFunctionsExtensionsType = typeof(MySqlDbFunctionsExtensions);
    private const string DateDiffDayMethod = nameof(MySqlDbFunctionsExtensions.DateDiffDay);
    private static readonly MemberExpression EfPropertyExpression = Property(null, typeof(EF), nameof(EF.Functions));

    /// <summary>
    /// Creates DateDiffDay filter over entity property and filter value
    /// </summary>
    /// <param name="filterBuilder">Instance of <see cref="IFilterBuilder{TEntity,TFilter}"/></param>
    /// <param name="dataExpression">Expression targeting entity property</param>
    /// <param name="filterFunc">Filtering delegate targeting property with details if filter should apply</param>
    /// <param name="operation">Operation defines which operations is applied over property and filter value</param>
    /// <param name="diffConstant">Optional diff constant to use in comparison</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    public static IFilterBuilder<TEntity, TFilter> IsDateDiffDay<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
        => filterBuilder.IsDateDiffFilter(DateDiffDayMethod, dataExpression, filterFunc, operation, diffConstant);
    
    private static IFilterBuilder<TEntity, TFilter> IsDateDiffFilter<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        string methodName,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc,
        DateDiffOperation operation,
        int diffConstant = 0)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter) != null;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter).GetValueOrDefault();
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var constantExpression = BasicExpressions.GetWrappedConstantExpression(filterValue);
            var callExpression = Call(
                DbFunctionsExtensionsType,
                methodName,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!,
                constantExpression);

            var operationExpressionFactory = GetOperationFactory(operation);
            var operationExpression = operationExpressionFactory(callExpression, Constant(diffConstant));
            return Lambda<Func<TEntity, bool>>(operationExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
    
    private static Func<Expression, Expression, Expression> GetOperationFactory(DateDiffOperation operation)
        => operation switch
        {
            DateDiffOperation.GreaterThan => GreaterThan,
            DateDiffOperation.LessThan => LessThan,
            DateDiffOperation.Equal => Equal,
            DateDiffOperation.NotEqual => (callExpr, constant) => Not(Equal(callExpr, constant)),
            DateDiffOperation.NotGreaterThan => (callExpr, constant) => Not(GreaterThan(callExpr, constant)),
            DateDiffOperation.NotLessThan => (callExpr, constant) => Not(LessThan(callExpr, constant)),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
}