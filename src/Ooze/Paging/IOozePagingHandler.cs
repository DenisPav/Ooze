using System.Linq;

namespace Ooze.Paging
{
    internal interface IOozePagingHandler
    {
        IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            int? page,
            int? pageSize);
    }
}
