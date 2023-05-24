using System.Linq.Expressions;

namespace Ooze.Typed.Paging;

internal interface IOozePagingHandler<TEntity>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions);

    IQueryable<TEntity> ApplyCursor<TAfter, TProperty>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TProperty>> cursorPropertyExpression,
        CursorPagingOptions<TAfter> pagingOptions);
}
