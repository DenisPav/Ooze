﻿using Ooze.Configuration;
using Ooze.Sorters;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ooze.Expressions.OozeExpressionCreator;

namespace Ooze.Filters
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
            //this might throw??? check and make sure
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
            //this might throw??? check and make sure
            var sorters = entityConfiguration.Sorters;
            var sorter = sorters.SingleOrDefault(sorter => string.Compare(sorter.Name, parsedSorter.Sorter, StringComparison.InvariantCultureIgnoreCase) == 0);

            if (sorter != null)
            {
                var sortExpression = SortExpression<TEntity>(query.Expression, sorter.Expression, sorter.Type, parsedSorter.Ascending, isFirst);

                return query.Provider
                    .CreateQuery<TEntity>(sortExpression);
            }
            else
            {
                var provider = customSorterProviders.SingleOrDefault(provider => string.Compare(provider.Name, parsedSorter.Sorter, StringComparison.InvariantCultureIgnoreCase) == 0);
                var providerQuery = isFirst
                    ? provider?.ApplySorter(query, parsedSorter.Ascending)
                    : provider?.ThenApplySorter((IOrderedQueryable<TEntity>)query, parsedSorter.Ascending);

                if (providerQuery != null)
                    return providerQuery;
            }

            return query;
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
}
