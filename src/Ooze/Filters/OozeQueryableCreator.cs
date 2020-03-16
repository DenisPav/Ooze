using Ooze.Configuration;
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

            if (filter != null)
            {
                var callExpr = FilterExpression<TEntity>(entityConfiguration, parsedFilter, filter, query.Expression, operationsMap[parsedFilter.Operation]);
                return query.Provider
                    .CreateQuery<TEntity>(callExpr);
            }
            else
            {
                var providerQuery = customFilterProviders.SingleOrDefault(provider => string.Equals(provider.Name, parsedFilter.Property, StringComparison.InvariantCultureIgnoreCase))
                       ?.ApplyFilter(query, parsedFilter);

                if (providerQuery != null)
                    return providerQuery;
            }

            return query;
        }
    }
}
