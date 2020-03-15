using System.Linq;

namespace Ooze.Filters
{
    public interface IOozeFilterHandler<TEntity>
    {
        IQueryable<TEntity> Handle(IQueryable<TEntity> query, string filters);
    }
}
