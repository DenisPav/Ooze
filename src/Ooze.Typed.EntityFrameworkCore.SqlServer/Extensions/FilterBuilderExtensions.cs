using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Expressions;
using Ooze.Typed.Filters;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.EntityFrameworkCore.SqlServer.Extensions;

public static class FilterBuilderExtensions
{
    private static readonly Type DbFunctionsExtensionsType = typeof(SqlServerDbFunctionsExtensions);
    private const string IsDateMethod = nameof(SqlServerDbFunctionsExtensions.IsDate);
    private const string IsNumericMethod = nameof(SqlServerDbFunctionsExtensions.IsNumeric);
    private const string DateDiffDayMethod = nameof(SqlServerDbFunctionsExtensions.DateDiffDay);
    private static readonly MemberExpression EfPropertyExpression = Property(null, typeof(EF), nameof(EF.Functions));
    
    public static IFilterBuilder<TEntity, TFilter> IsDate<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, bool?> filterFunc)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter).GetValueOrDefault() == true;
        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter).GetValueOrDefault();
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var callExpression = Call(
                DbFunctionsExtensionsType,
                IsDateMethod,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!);
            Expression notExpression = filterValue == true
                ? callExpression
                : Not(callExpression);

            return Lambda<Func<TEntity, bool>>(notExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
    
    public static IFilterBuilder<TEntity, TFilter> IsNumeric<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, bool?> filterFunc)
    {
        bool FilterShouldRun(TFilter filter) => filterFunc(filter).GetValueOrDefault() == true;
        Expression<Func<TEntity, bool>> FilterExpressionFactory(TFilter filter)
        {
            var filterValue = filterFunc(filter).GetValueOrDefault();
            var memberAccessExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
            var parameterExpression = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
            var callExpression = Call(
                DbFunctionsExtensionsType,
                IsNumericMethod,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!);
            Expression notExpression = filterValue == true
                ? callExpression
                : Not(callExpression);

            return Lambda<Func<TEntity, bool>>(notExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
    
    public static IFilterBuilder<TEntity, TFilter> DateDiffDayEqual<TEntity, TFilter>(
        this IFilterBuilder<TEntity, TFilter> filterBuilder,
        Expression<Func<TEntity, DateTime?>> dataExpression,
        Func<TFilter, DateTime?> filterFunc)
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
                DateDiffDayMethod,
                Type.EmptyTypes,
                EfPropertyExpression,
                memberAccessExpression!,
                constantExpression);
            var equalExpression = Equal(callExpression, Constant(0));

            return Lambda<Func<TEntity, bool>>(equalExpression, parameterExpression);
        }

        filterBuilder.Add(FilterShouldRun, FilterExpressionFactory);
        return filterBuilder;
    }
}