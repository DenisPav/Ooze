using System.Linq;

namespace Ooze.Filters
{
    public interface IOozeFilterProvider<TEntity> : IOozeProvider
        where TEntity : class
    {
        IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, FilterParserResult filter);
    }
}
