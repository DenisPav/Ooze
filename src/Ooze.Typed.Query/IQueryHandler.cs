namespace Ooze.Typed.Query;

internal interface IQueryHandler<TEntity>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        string queryDefinition);
}