using Ooze.Filters;
using Ooze.Sorters;
using System.Linq;

namespace Ooze
{
    public class OozeResolver : IOozeResolver
    {
        readonly IOozeFilterHandler _filterHandler;
        readonly IOozeSorterHandler _sorterHandler;

        public OozeResolver(
            IOozeFilterHandler filterHandler,
            IOozeSorterHandler sorterHandler)
        {
            _filterHandler = filterHandler;
            _sorterHandler = sorterHandler;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, OozeModel model)
        {
            query = _sorterHandler.Handle(query, model.Sorters);
            query = _filterHandler.Handle(query, model.Filters);

            return query;
        }


    }
}
