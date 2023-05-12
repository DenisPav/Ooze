using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Expressions;
using Ooze.Typed.Filters;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.Extensions;

public static class FilterBuilderExtensions
{
    private static readonly Type DbFunctionsExtensionsType = typeof(DbFunctionsExtensions);
    private const string LikeMethod = nameof(DbFunctionsExtensions.Like);
    private static readonly MemberExpression EfPropertyExpression = Property(null, typeof(EF), nameof(EF.Functions));
    
    public static IFilterBuilder<TEntity, TFilter> Like<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc)
    {
        bool FilterShouldRun(TFilter filter) => string.IsNullOrEmpty(filterFunc(filter)) == false;
        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter);
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var constantExpression = BasicExpressions.GetWrappedConstantExpression(filterValue);
            var callExpression = Call(
                DbFunctionsExtensionsType, 
                LikeMethod,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!,
                constantExpression);

            return Lambda<Func<TEntity, bool>>(callExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
}