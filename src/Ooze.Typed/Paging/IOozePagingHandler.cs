namespace Ooze.Typed.Paging;

internal interface IOozePagingHandler<TEntity>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions);
}
