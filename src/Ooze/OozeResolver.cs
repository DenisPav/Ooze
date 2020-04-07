using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Query;
using Ooze.Sorters;
using Ooze.Validation;
using System;
using System.Linq;
using static System.Linq.Expressions.Expression;

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
            var (sortersValid, filtersValid, queryValid, fieldsValid) = _modelValidator.Validate(model, _config.UseSelections);

            //apply query if it is present
            if (queryValid)
            {
                return _queryHandler.Handle(query, model.Query);
            }

            //in other case try defaults
            if (sortersValid)
            {
                query = _sorterHandler.Handle(query, model.Sorters);
            }

            if (filtersValid)
            {
                query = _filterHandler.Handle(query, model.Filters);
            }

            if (_config.UseSelections && fieldsValid)
            {
                var paramExpr = Parameter(
                    typeof(TEntity),
                    typeof(TEntity).Name
                );

                var bindExpressions = typeof(TEntity).GetProperties()
                    .Where(prop => model.Fields.Contains(prop.Name, StringComparison.InvariantCultureIgnoreCase))
                    .Select(prop => Bind(prop, MakeMemberAccess(paramExpr, prop)));

                var lambda = Lambda<Func<TEntity, TEntity>>(
                    MemberInit(
                        New(
                            typeof(TEntity).GetConstructors().First()
                        ),
                        bindExpressions
                    ),
                    paramExpr
                );

                query = query.Select(lambda);
            }

            return query;
        }
    }
}
