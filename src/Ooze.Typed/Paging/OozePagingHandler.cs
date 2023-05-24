using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Ooze.Typed.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Typed.Paging;

internal class OozePagingHandler<TEntity> : IOozePagingHandler<TEntity>
{
    private readonly ILogger<OozePagingHandler<TEntity>> _log;

    public OozePagingHandler(ILogger<OozePagingHandler<TEntity>> log) => _log = log;

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions)
    {
        _log.LogDebug("Applying paging options! Page: [{page}], Page size: [{size}]", pagingOptions.Page,
            pagingOptions.Size);

        return query.Skip(pagingOptions.Page * pagingOptions.Size)
            .Take(pagingOptions.Size);
    }

    public IQueryable<TEntity> ApplyCursor<TAfter, TProperty>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter> pagingOptions)
    {
        _log.LogDebug("Applying cursor paging options! After: [{after}], Page size: [{size}]", pagingOptions.After,
            pagingOptions.Size);

        var memberAccessExpression = BasicExpressions.GetMemberExpression(cursorPropertyExpression.Body);
        var constant = BasicExpressions.GetWrappedConstantExpression(pagingOptions.After);
        var parameter = BasicExpressions.ExtractParameterExpression(memberAccessExpression);
        var cursorExpression = GreaterThan(memberAccessExpression, constant);

        var lambdaExpression = BasicExpressions.GetLambdaExpression<TEntity>(cursorExpression, parameter);

        return query.Where(lambdaExpression)
            .Take(pagingOptions.Size);
    }
}