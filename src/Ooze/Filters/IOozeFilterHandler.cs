using System.Linq;

namespace Ooze.Filters
{
    public interface IOozeFilterHandler
    {
        IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string filters);
    }
}
