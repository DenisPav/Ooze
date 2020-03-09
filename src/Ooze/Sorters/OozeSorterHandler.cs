using Ooze.Configuration;
using System;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Sorters
{
    public class OozeSorterHandler : IOozeSorterHandler
    {
        const string OrderBy = nameof(OrderBy);
        const string ThenBy = nameof(ThenBy);
        const string ThenByDescending = nameof(ThenByDescending);
        const string OrderByDescending = nameof(OrderByDescending);
        const char NegativeOrderChar = '-';

        static readonly Type _queryableType = typeof(Queryable);

        readonly OozeConfiguration _config;

        public OozeSorterHandler(
            OozeConfiguration config)
        {
            _config = config;
        }

        public IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string sorters)
        {
            var entity = typeof(TEntity);
            var configuration = _config.EntityConfigurations[entity];

            var modelSorters = sorters.Split(',').Select(sorter => sorter.Trim())
                .Select(sorter => new
                {
                    Ascending = !sorter.StartsWith(NegativeOrderChar) ? true : false,
                    Sorter = sorter.StartsWith(NegativeOrderChar) ? new string(sorter.Skip(1).ToArray()) : sorter
                })
                .ToList();

            var appliedSorters = modelSorters.Join(
                configuration.Sorters,
                x => x.Sorter,
                x => x.Name,
                (x, y) => (y.Name, y.Expression, y.Type, x.Ascending),
                StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            for (int i = 0; i < appliedSorters.Count(); i++)
            {
                var expr = query.Expression;
                var sorter = appliedSorters[i];
                MethodCallExpression callExpr;
                var quoteExpr = Quote(sorter.Item2);
                var typings = new Type[] { entity, sorter.Item3 };

                if (i == 0)
                {
                    var method = sorter.Ascending ? OrderBy : OrderByDescending;
                    callExpr = Call(_queryableType, method, typings, expr, quoteExpr);
                }
                else
                {
                    var method = sorter.Ascending ? ThenBy : ThenByDescending;
                    callExpr = Call(_queryableType, method, typings, expr, quoteExpr);
                }

                query = query.Provider
                    .CreateQuery<TEntity>(callExpr);
            }

            return query;
        }
    }
}
