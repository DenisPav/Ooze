using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Query;
using Ooze.Sorters;
using Ooze.Validation;
using System.Linq;

namespace Ooze
{
    internal class OozeResolver : IOozeResolver
    {
        readonly IOozeSorterHandler _sorterHandler;
        readonly IOozeFilterHandler _filterHandler;
        readonly IOozeQueryHandler _queryHandler;
        readonly OozeConfiguration _config;

        static readonly OozeModelValidator _modelValidator = new OozeModelValidator();

        public OozeResolver(
            IOozeSorterHandler sorterHandler,
            IOozeFilterHandler filterHandler,
            IOozeQueryHandler queryHandler,
            OozeConfiguration config)
        {
            _sorterHandler = sorterHandler;
            _filterHandler = filterHandler;
            _queryHandler = queryHandler;
            _config = config;
        }

        public IQueryable<TEntity> Apply<TEntity>(
            IQueryable<TEntity> query,
            OozeModel model)
            where TEntity : class
        {
            if (!_config.EntityConfigurations.ContainsKey(typeof(TEntity)))
                return query;

            return ApplyOoze(query, model);
        }

        IQueryable<TEntity> ApplyOoze<TEntity>(
            IQueryable<TEntity> query,
            OozeModel model)
            where TEntity : class
        {
            var (sortersValid, filtersValid, queryValid) = _modelValidator.Validate(model);

            //if (sortersValid)
            //{
            //    query = _sorterHandler.Handle(query, model.Sorters);
            //}

            //if (filtersValid)
            //{
            //    query = _filterHandler.Handle(query, model.Filters);
            //}

            if (queryValid)
            {
                query = _queryHandler.Handle(query, model.Query);
            }

            return query;
        }
    }
}
