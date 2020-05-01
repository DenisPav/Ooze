﻿using Microsoft.Extensions.Logging;
using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Paging;
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
        readonly IOozePagingHandler _pagingHandler;
        readonly OozeConfiguration _config;
        readonly ILogger<OozeResolver> _log;

        static readonly OozeModelValidator _modelValidator = new OozeModelValidator();

        public OozeResolver(
            IOozeSorterHandler sorterHandler,
            IOozeFilterHandler filterHandler,
            IOozeQueryHandler queryHandler,
            IOozeSelectionHandler selectionHandler,
            IOozePagingHandler pagingHandler,
            OozeConfiguration config,
            ILogger<OozeResolver> log)
        {
            _sorterHandler = sorterHandler;
            _filterHandler = filterHandler;
            _queryHandler = queryHandler;
            _selectionHandler = selectionHandler;
            _pagingHandler = pagingHandler;
            _config = config;
            _log = log;
        }

        public IQueryable<TEntity> Apply<TEntity>(
            IQueryable<TEntity> query,
            OozeModel model)
            where TEntity : class
        {
            _log.LogDebug("Applying changes to passed IQueryable instance");

            var hasConfig = HasEntityConfiguration<TEntity>();
            var validationResult = _modelValidator.Validate(model);

            if (hasConfig)
            {
                query = ApplyStandard(query, model, validationResult);
            }

            //apply query even if config isn't present
            if (_config.UseSelections && validationResult.FieldsValid)
            {
                query = _selectionHandler.Handle(query, model.Fields);
            }

            if (_config.UsePaging)
            {
                query = _pagingHandler.Handle(query, model.Page, model.PageSize);
            }

            _log.LogDebug("Final queryable expression: {expression}", query.Expression.ToString());
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
