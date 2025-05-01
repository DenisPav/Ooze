namespace Ooze.Typed.Query;

internal interface IQueryLanguageHandler<TEntity>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        string queryDefinition);
}