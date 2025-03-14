namespace Ooze.Typed.Query;

public interface IQueryLanguageOperationResolver
    : IOperationResolver
{
    IQueryable<TEntity> FilterWithQueryLanguage<TEntity>(
        IQueryable<TEntity> queryable,
        string? query);
}

public interface IQueryLanguageOperationResolver<TEntity, TFilters, TSorterts>
    : IOperationResolver<TEntity, TFilters, TSorterts>
{
    IQueryLanguageOperationResolver<TEntity, TFilters, TSorterts> FilterWithQueryLanguage(string query);
}