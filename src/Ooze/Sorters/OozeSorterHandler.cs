using Ooze.Configuration;
using Superpower;
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

        readonly IOozeCustomProviderProvider _customProviderProvider;
        readonly OozeConfiguration _config;

        public OozeSorterHandler(
            IOozeCustomProviderProvider customProviderProvider,
            OozeConfiguration config)
        {
            _customProviderProvider = customProviderProvider;
            _config = config;
        }

        public IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string sorters)
            where TEntity : class
        {
            var entity = typeof(TEntity);
            var configuration = _config.EntityConfigurations[entity];
            var customProviders = _customProviderProvider.SortersFor<TEntity>();

            var parsedSorters = GetParsedSorters(sorters).ToList();
            var appliedSorters = GetAppliedSorters(configuration, parsedSorters);

            for (int i = 0; i < parsedSorters.Count(); i++)
            {
                var queryExpression = query.Expression;
                var parsedSorter = parsedSorters[i];
                //not ThenBy call
                var isFirst = i == 0;

                var configSorter = appliedSorters.SingleOrDefault(sorter => string.Compare(sorter.Name, parsedSorter.Sorter, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (configSorter != null)
                {
                    var sortExpression = CreateSortExpression(entity, queryExpression, configSorter.Expression, configSorter.Type, parsedSorter.Ascending, isFirst);

                    query = query.Provider
                        .CreateQuery<TEntity>(sortExpression);
                }
                else
                {
                    var provider = customProviders.SingleOrDefault(provider => string.Compare(provider.Name, parsedSorter.Sorter, StringComparison.InvariantCultureIgnoreCase) == 0);
                    var providerQuery = isFirst
                        ? provider?.ApplySorter(query, parsedSorter.Ascending)
                        : provider?.ThenApplySorter((IOrderedQueryable<TEntity>)query, parsedSorter.Ascending);

                    if (providerQuery != null)
                        query = providerQuery;
                }
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
            var typings = new[] { entityType, sorterType };
            var quoteExpr = Quote(sorterExpression);

            var method = isFirst
                ? ascending ? OrderBy : OrderByDescending
                : ascending ? ThenBy : ThenByDescending;

            return Call(_queryableType, method, typings, queryExpression, quoteExpr);
        }

        IEnumerable<ParsedExpressionDefinition> GetAppliedSorters(
            OozeEntityConfiguration configuration,
            IEnumerable<SorterParserResult> parsedSorters)
        {
            var distinctSorters = parsedSorters.Select(sorter => sorter.Sorter)
                .Distinct();

            return distinctSorters.Join(
                configuration.Sorters,
                parsedSorter => parsedSorter,
                configuredSorter => configuredSorter.Name,
                (parsedSorter, configuredSorter) => configuredSorter,
                StringComparer.InvariantCultureIgnoreCase)
                .ToList();
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
                });
        }
    }
}
