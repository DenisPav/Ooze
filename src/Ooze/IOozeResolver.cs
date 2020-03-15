using System.Linq;

namespace Ooze
{
    public interface IOozeResolver
    {
        IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, OozeModel model)
            where TEntity : class;
    }
}
