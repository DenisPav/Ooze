using System.Linq;

namespace Ooze.Sorters
{
    public interface IOozeSorterProvider<TEntity> : IOozeProvider
        where TEntity : class
    {
        IQueryable<TEntity> ApplySorter(IQueryable<TEntity> query, bool ascending);
        IQueryable<TEntity> ThenApplySorter(IOrderedQueryable<TEntity> query, bool ascending);
    }
}
