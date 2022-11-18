namespace Ooze.Typed.Queries;

public interface IOozeQueryHandler<TEntity>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        string queryDefinition);
}