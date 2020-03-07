using System.Linq;

namespace Ooze.Sorters
{
    public interface IOozeSorterHandler
    {
        IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string sorters);
    }
}
