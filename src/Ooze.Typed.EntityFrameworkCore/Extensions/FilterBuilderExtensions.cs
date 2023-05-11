using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Filters;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.Extensions;

public static class FilterBuilderExtensions
{
    public static IFilterBuilder<TEntity, TFilter> Like<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc)
    {
        bool FilterShouldRun(TFilter filter) => string.IsNullOrEmpty(filterFunc(filter)) == false;
        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var memberAccessExpression = dataExpression.Body switch
            {
                UnaryExpression unaryExpression => unaryExpression.Operand as MemberExpression,
                MemberExpression memberExpression => memberExpression,
                _ => throw new Exception("not handled yet")
            };

            var callExpression = Call(
                typeof(DbFunctionsExtensions), 
                nameof(DbFunctionsExtensions.Like),
                Type.EmptyTypes,
                Property(null, typeof(EF), nameof(EF.Functions)),
                memberAccessExpression!,
                Constant(filterFunc(filter)));
            
            var intermediateExpression = memberAccessExpression.Expression;
            while (intermediateExpression is MemberExpression member)
                intermediateExpression = member.Expression;

            return Lambda<Func<TEntity, bool>>(callExpression, intermediateExpression as ParameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
}