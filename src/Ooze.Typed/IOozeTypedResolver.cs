namespace Ooze.Typed;

public interface IOozeTypedResolver
{
    IQueryable<TEntity> Filter<TEntity, TFilters>(IQueryable<TEntity> query, TFilters filters);
}
