using System.Linq;

namespace Ooze.Sorters
{
    public interface IOozeSorterHandler<TEntity>
    {
        IQueryable<TEntity> Handle(IQueryable<TEntity> query, string sorters);
    }
}
