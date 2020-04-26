using System.Linq;

namespace Ooze.Paging
{
    public interface IOozePagingHandler
    {
        IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            int? page,
            int? pageSize);
    }
}
