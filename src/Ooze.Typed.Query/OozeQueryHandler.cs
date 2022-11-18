using Ooze.Typed.Queries;

namespace Ooze.Typed.Query;

public class OozeQueryHandler<TEntity> : IOozeQueryHandler<TEntity>
{
    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        string queryDefinition)
    {
        throw new NotImplementedException();
    }
}