using System.Linq;

namespace Ooze.Filters
{
    public interface IOozeFilterProvider<TEntity>
        where TEntity : class
    {
        string Name { get; }
        IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, FilterParserResult filter);
    }
}
