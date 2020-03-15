using Microsoft.Extensions.DependencyInjection;
using Ooze.Filters;
using Ooze.Sorters;
using Ooze.Validation;
using System;
using System.Linq;

namespace Ooze
{
    internal class OozeResolver : IOozeResolver
    {
        readonly IOozeSorterHandler _sorterHandler;
        readonly IOozeFilterHandler _filterHandler;
        
        static readonly OozeModelValidator _modelValidator = new OozeModelValidator();

        public OozeResolver(
            IOozeSorterHandler sorterHandler,
            IOozeFilterHandler filterHandler)
        {
            _sorterHandler = sorterHandler;
            _filterHandler = filterHandler;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, OozeModel model)
            where TEntity : class
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
