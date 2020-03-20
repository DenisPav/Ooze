using Ooze.Configuration;
using Ooze.Expressions;
using Ooze.Filters;
using Ooze.Sorters;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ooze.Expressions.OozeExpressionCreator;
using static Ooze.Filters.OozeParserCreator;

namespace Ooze
{
    internal static class OozeQueryableCreator
    {
        public static IQueryable<TEntity> ForFilter<TEntity>(
            IQueryable<TEntity> query,
            OozeEntityConfiguration entityConfiguration,
            FilterParserResult parsedFilter,
            IEnumerable<IOozeFilterProvider<TEntity>> customFilterProviders,
            IReadOnlyDictionary<string, Operation> operationsMap)
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            var filters = entityConfiguration.Filters;
            var filter = filters.SingleOrDefault(configFilter => string.Equals(configFilter.Name, parsedFilter.Property, StringComparison.InvariantCultureIgnoreCase));

            return filter != null
                ? ApplyConfigurationFilter(query, entityConfiguration, parsedFilter, operationsMap, filter)
                : ApplyCustomFilter(query, customFilterProviders, parsedFilter);
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
            IReadOnlyDictionary<string, Operation> operationsMap)
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            var filters = entityConfiguration.Filters;

            var mappedParts = parsedQueryParts.Select(part => new QueryFilterOperation
            {
                Filter = filters.SingleOrDefault(configFilter => string.Equals(configFilter.Name, part.Property, StringComparison.InvariantCultureIgnoreCase)),
                OperationFactory = operationsMap[part.Operation],
                QueryPart = part
            });

            var callExpr = OozeExpressionCreator.QueryPartExpression<TEntity>(entityConfiguration, mappedParts, query.Expression);
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

        static IQueryable<TEntity> ApplyConfigurationFilter<TEntity>(
            IQueryable<TEntity> query,
            OozeEntityConfiguration entityConfiguration,
            FilterParserResult parsedFilter,
            IReadOnlyDictionary<string, Operation> operationsMap,
            ParsedExpressionDefinition filter)
            where TEntity : class
        {
            var callExpr = FilterExpression<TEntity>(entityConfiguration, parsedFilter, filter, query.Expression, operationsMap[parsedFilter.Operation]);
            return query.Provider
                .CreateQuery<TEntity>(callExpr);
        }

        static IQueryable<TEntity> ApplyCustomFilter<TEntity>(
            IQueryable<TEntity> query,
            IEnumerable<IOozeFilterProvider<TEntity>> filterProviders,
            FilterParserResult parsedFilter)
            where TEntity : class
        {
            var providerQuery = filterProviders.SingleOrDefault(provider => string.Equals(provider.Name, parsedFilter.Property, StringComparison.InvariantCultureIgnoreCase))
                       ?.ApplyFilter(query, parsedFilter) ?? query;

            return providerQuery;
        }
    }

    internal class QueryFilterOperation
    {
        public ParsedExpressionDefinition Filter { get; set; }
        public Operation OperationFactory { get; set; }
        public QueryParserResult QueryPart { get; set; }
    }
}
