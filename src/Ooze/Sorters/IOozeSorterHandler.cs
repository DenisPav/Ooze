using System.Linq;

namespace Ooze.Sorters
{
    internal interface IOozeSorterHandler
    {
        IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string sorters)
            where TEntity : class;
    }
}
