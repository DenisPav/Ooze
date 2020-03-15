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
        readonly IServiceProvider _provider;
        static readonly OozeModelValidator _modelValidator = new OozeModelValidator();

        public OozeResolver(
            IServiceProvider provider)
        {
            _provider = provider;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, OozeModel model)
        {
            var (sortersValid, filtersValid) = _modelValidator.Validate(model);

            if (sortersValid)
            {
                query = _provider.GetRequiredService<IOozeSorterHandler<TEntity>>().Handle(query, model.Sorters);
            }

            if (filtersValid)
            {
                query = _provider.GetRequiredService<IOozeFilterHandler<TEntity>>().Handle(query, model.Filters);
            }

            return query;
        }
    }
}
