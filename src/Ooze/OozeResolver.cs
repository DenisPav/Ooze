using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Query;
using Ooze.Selections;
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
        readonly IOozeSelectionHandler _selectionHandler;
        readonly OozeConfiguration _config;

        static readonly OozeModelValidator _modelValidator = new OozeModelValidator();

        public OozeResolver(
            IOozeSorterHandler sorterHandler,
            IOozeFilterHandler filterHandler,
            IOozeQueryHandler queryHandler,
            IOozeSelectionHandler selectionHandler,
            OozeConfiguration config)
        {
            _sorterHandler = sorterHandler;
            _filterHandler = filterHandler;
            _queryHandler = queryHandler;
            _selectionHandler = selectionHandler;
            _config = config;
        }

        public IQueryable<TEntity> Apply<TEntity>(
            IQueryable<TEntity> query,
            OozeModel model)
            where TEntity : class
        {
            var hasConfig = HasEntityConfiguration<TEntity>();
            var validationResult = _modelValidator.Validate(model, _config.UseSelections);

            if (hasConfig)
            {
                query = ApplyStandard(query, model, validationResult);
            }

            //apply query even if config isn't present
            if (_config.UseSelections && validationResult.FieldsValid)
            {
                query = _selectionHandler.Handle(query, model.Fields);
            }

            return query;
        }

        IQueryable<TEntity> ApplyStandard<TEntity>(
            IQueryable<TEntity> query,
            OozeModel model,
            OozeModelValidationResult validationResult)
            where TEntity : class
        {
            //apply query if it is present
            if (validationResult.QueryValid)
            {
                return _queryHandler.Handle(query, model.Query);
            }

            //in other case try defaults
            if (validationResult.SortersValid)
            {
                query = _sorterHandler.Handle(query, model.Sorters);
            }

            if (validationResult.FiltersValid)
            {
                query = _filterHandler.Handle(query, model.Filters);
            }

            return query;
        }

        bool HasEntityConfiguration<TEntity>() => _config.EntityConfigurations.ContainsKey(typeof(TEntity));
    }
}
