using Ooze.Configuration;
using Ooze.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ooze.Expressions.OozeExpressionCreator;

namespace Ooze
{
    internal static class OozeQueryableCreator
    {
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
    }
}
