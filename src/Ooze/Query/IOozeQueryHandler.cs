using System.Linq;

namespace Ooze.Query
{
    public interface IOozeQueryHandler
    {
        IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string modelQuery)
            where TEntity : class;
    }
}
