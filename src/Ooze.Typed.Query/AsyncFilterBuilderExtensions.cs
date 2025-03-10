using System.Linq.Expressions;
using Ooze.Typed.Filters.Async;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.Query;

/// <summary>
/// Query language extensions for AsyncFilterBuilder
/// </summary>
public static class AsyncFilterBuilderExtensions
{
    //TODO: docs
    public static IAsyncFilterBuilder<TEntity, TFilter> Query<TEntity, TFilter>(
        this IAsyncFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string?>> dataExpression,
        Func<TFilter, string?> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter) != null;

        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            // var filterValue = filterFunc(filter).GetValueOrDefault();
            // var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            // var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            // var constantExpression = BasicExpressions.GetWrappedConstantExpression(filterValue);
            // var callExpression = Call(
            //     Shared.DbFunctionsExtensionsType,
            //     methodName,
            //     Type.EmptyTypes,
            //     Shared.EfPropertyExpression,
            //     memberAccessExpression!,
            //     constantExpression);
            //
            // var operationExpressionFactory = GetOperationFactory(operation);
            // var operationExpression = operationExpressionFactory(callExpression, Constant(diffConstant));
            // return Lambda<Func<TEntity, bool>>(operationExpression, parameterExpression);

            return null;
        }

        shouldRun ??= FilterShouldRun;
        filterBuilder.Add(shouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
}