using System.Linq;

namespace Ooze.Filters
{
    internal interface IOozeFilterHandler
    {
        IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string filters)
            where TEntity : class;
    }
}
