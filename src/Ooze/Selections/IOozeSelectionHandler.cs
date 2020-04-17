using System.Linq;

namespace Ooze.Selections
{
    internal interface IOozeSelectionHandler
    {
        IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string fields)
            where TEntity : class;
    }
}
