using Ooze.Configuration;
using Ooze.Filters;
using Ooze.Query;
using Ooze.Sorters;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ooze.Expressions.OozeExpressionCreator;

namespace Ooze
{
    internal static class OozeQueryableCreator
    {
        public static IQueryable<TEntity> ForFilter<TEntity>(
            IQueryable<TEntity> query,
            FilterParserResult parsedFilter,
            IEnumerable<IOozeFilterProvider<TEntity>> filterProviders)
            where TEntity : class
        {
            var filter = filterProviders.SingleOrDefault(configFilter => string.Equals(configFilter.Name, parsedFilter.Property, StringComparison.InvariantCultureIgnoreCase));
            return filter.ApplyFilter(query, parsedFilter);
        }

        public static IQueryable<TEntity> ForSorter<TEntity>(
            IQueryable<TEntity> query,
            OozeEntityConfiguration entityConfiguration,
            SorterParserResult parsedSorter,
            IEnumerable<IOozeSorterProvider<TEntity>> customSorterProviders,
            bool isFirst)
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            var sorters = entityConfiguration.Sorters;
            var sorter = sorters.SingleOrDefault(sorter => string.Compare(sorter.Name, parsedSorter.Sorter, StringComparison.InvariantCultureIgnoreCase) == 0);

            return sorter != null
                ? ApplyConfigurationSorter(query, parsedSorter, isFirst, sorter)
                : ApplyCustomSorter(query, customSorterProviders, parsedSorter, isFirst);
        }

        public static IQueryable<TEntity> ForQuery<TEntity>(
            IQueryable<TEntity> query,
            OozeEntityConfiguration entityConfiguration,
            QueryParserResult[] parsedQueryParts,
            //IEnumerable<IOozeFilterProvider<TEntity>> customFilterProviders,
            IReadOnlyDictionary<string, Operation> operationsMap,
            IReadOnlyDictionary<string, Operation> logicalOperationsMap)
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            var filters = entityConfiguration.Filters;

            var mappedQueryParts = parsedQueryParts.Select(part => new QueryFilterOperation
            {
                Filter = filters.SingleOrDefault(configFilter => string.Equals(configFilter.Name, part.Property, StringComparison.InvariantCultureIgnoreCase)),
                OperationFactory = operationsMap[part.Operation],
                LogicalOperationFactory = logicalOperationsMap.TryGetValue(part.LogicalOperation, out var factory) ? factory : null,
                QueryPart = part
            });

            var callExpr = QueryExpression<TEntity>(entityConfiguration, mappedQueryParts, query.Expression);
            return query.Provider.CreateQuery<TEntity>(callExpr);
        }

        static IQueryable<TEntity> ApplyConfigurationSorter<TEntity>(
            IQueryable<TEntity> query,
            SorterParserResult parsedSorter,
            bool isFirst,
            ParsedExpressionDefinition sorter)
            where TEntity : class
        {
            var sortExpression = SortExpression<TEntity>(query.Expression, sorter.Expression, sorter.Type, parsedSorter.Ascending, isFirst);

            return query.Provider
                .CreateQuery<TEntity>(sortExpression);
        }

        static IQueryable<TEntity> ApplyCustomSorter<TEntity>(
            IQueryable<TEntity> query,
            IEnumerable<IOozeSorterProvider<TEntity>> sorterProviders,
            SorterParserResult parsedSorter,
            bool isFirst)
            where TEntity : class
        {
            var provider = sorterProviders.SingleOrDefault(provider => string.Compare(provider.Name, parsedSorter.Sorter, StringComparison.InvariantCultureIgnoreCase) == 0);
            var providerQuery = (isFirst
                ? provider?.ApplySorter(query, parsedSorter.Ascending)
                : provider?.ThenApplySorter((IOrderedQueryable<TEntity>)query, parsedSorter.Ascending))
                ?? query;

            return providerQuery;
        }
    }
}
