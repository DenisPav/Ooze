using Ooze.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Sorters
{
    internal class OozeSorterHandler : IOozeSorterHandler
    {
        const string OrderBy = nameof(OrderBy);
        const string ThenBy = nameof(ThenBy);
        const string ThenByDescending = nameof(ThenByDescending);
        const string OrderByDescending = nameof(OrderByDescending);
        const char _negativeOrderChar = '-';

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

            var parsedSorters = GetParsedSorters(sorters);
            var appliedSorters = GetAppliedSorters(configuration, parsedSorters)
                .ToList();

            for (int i = 0; i < appliedSorters.Count(); i++)
            {
                var queryExpression = query.Expression;
                var (_, sorterExpression, sorterType, ascending) = appliedSorters[i];
                var sortExpression = CreateSortExpression(entity, queryExpression, sorterExpression, sorterType, ascending, i == 0);

                query = query.Provider
                    .CreateQuery<TEntity>(sortExpression);
            }

            return query;
        }

        MethodCallExpression CreateSortExpression(
            Type entityType,
            Expression queryExpression,
            Expression sorterExpression,
            Type sorterType,
            bool ascending,
            bool isFirst = true)
        {
            var typings = new [] { entityType, sorterType };
            var quoteExpr = Quote(sorterExpression);

            var method = isFirst
                ? ascending ? OrderBy : OrderByDescending
                : ascending ? ThenBy : ThenByDescending;

            return Call(_queryableType, method, typings, queryExpression, quoteExpr);
        }

        IEnumerable<(string Name, Expression Expression, Type Type, bool Ascending)> GetAppliedSorters(
            OozeEntityConfiguration configuration,
            IEnumerable<SorterParserResult> parsedSorters)
        {
            return parsedSorters.Join(
                configuration.Sorters,
                parsedSorter => parsedSorter.Sorter,
                configuredSorter => configuredSorter.Name,
                (parsedSorter, configuredSorter) => (configuredSorter.Name, configuredSorter.Expression, configuredSorter.Type, parsedSorter.Ascending),
                StringComparer.InvariantCultureIgnoreCase);
        }

        IEnumerable<SorterParserResult> GetParsedSorters(
            string sorters)
        {
            return sorters.Split(',')
                .Select(sorter => sorter.Trim())
                .Select(sorter => new SorterParserResult
                {
                    Ascending = !sorter.StartsWith(_negativeOrderChar) ? true : false,
                    Sorter = sorter.StartsWith(_negativeOrderChar) ? new string(sorter.Skip(1).ToArray()) : sorter
                })
                .ToList();
        }
    }
}
