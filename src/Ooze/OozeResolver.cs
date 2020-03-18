using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Sorters;
using Ooze.Validation;
using System.Linq;

namespace Ooze
{
    internal class OozeResolver : IOozeResolver
    {
        readonly IOozeSorterHandler _sorterHandler;
        readonly IOozeFilterHandler _filterHandler;
        readonly OozeConfiguration _config;

        static readonly OozeModelValidator _modelValidator = new OozeModelValidator();

        public OozeResolver(
            IOozeSorterHandler sorterHandler,
            IOozeFilterHandler filterHandler,
            OozeConfiguration config)
        {
            _sorterHandler = sorterHandler;
            _filterHandler = filterHandler;
            _config = config;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, OozeModel model)
            where TEntity : class
        {
            if (!_config.EntityConfigurations.ContainsKey(typeof(TEntity)))
                return query;

            return ApplyOoze(query, model);
        }

        private IQueryable<TEntity> ApplyOoze<TEntity>(IQueryable<TEntity> query, OozeModel model) where TEntity : class
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
