using Ooze.Filters;
using Ooze.Sorters;
using Ooze.Validation;
using System.Linq;

namespace Ooze
{
    public class OozeResolver : IOozeResolver
    {
        readonly IOozeFilterHandler _filterHandler;
        readonly IOozeSorterHandler _sorterHandler;

        static readonly OozeModelValidator _modelValidator = new OozeModelValidator();

        public OozeResolver(
            IOozeFilterHandler filterHandler,
            IOozeSorterHandler sorterHandler)
        {
            _filterHandler = filterHandler;
            _sorterHandler = sorterHandler;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, OozeModel model)
        {
            var (sortersValid, filtersValid) = _modelValidator.Validate(model);

            if (sortersValid)
            {
                query = _sorterHandler.Handle(query, model.Sorters);
            }

            if (filtersValid)
            {
                query = _filterHandler.Handle(query, model.Filters);
            }

            return query;
        }


    }
}
