namespace Ooze.Typed.Paging;

internal class OozePagingHandler<TEntity> : IOozePagingHandler<TEntity>
{
    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions)
    {
        return query.Skip(pagingOptions.Page * pagingOptions.Size)
            .Take(pagingOptions.Size);
    }
}
