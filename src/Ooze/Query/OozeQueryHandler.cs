using Microsoft.Extensions.Logging;
using Ooze.Configuration;
using Ooze.Parsers;
using Superpower;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ooze.Expressions.OozeExpressionCreator;

namespace Ooze.Query
{
    internal class OozeQueryHandler : IOozeQueryHandler
    {
        readonly OozeConfiguration _config;
        readonly ILogger<OozeQueryHandler> _log;

        public OozeQueryHandler(
            OozeConfiguration config,
            ILogger<OozeQueryHandler> log)
        {
            _config = config;
            _log = log;
        }

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string modelQuery) where TEntity : class
        {
            _log.LogDebug("Running query language IQueryable updates");

            var entityType = typeof(TEntity);
            var entityConfiguration = _config.EntityConfigurations[entityType];
            var parser = CreateParser(entityConfiguration);

            var parsed = parser.TryParse(modelQuery);
            if (!parsed.HasValue)
            {
                throw new OozeQueryException("Query parsing failed");
            }

            var mappedQueryParts = MapQueryParts(parsed.Value, entityConfiguration.Filters);
            var expression = QueryExpression<TEntity>(entityConfiguration, mappedQueryParts, query.Expression);

            query = query.Provider
                .CreateQuery<TEntity>(expression);

            return query;
        }

        TextParser<QueryParserResult[]> CreateParser(
            OozeEntityConfiguration entityConfiguration)
        {
            _log.LogDebug("Creating Query language parser");

            var filterNames = entityConfiguration.Filters
                .Select(filter => filter.Name);
            var operations = _config.OperationsMap
                .Keys;
            var logicalOperations = _config.LogicalOperationMap
                .Keys;

            return OozeParserCreator.QueryParser(filterNames, operations, logicalOperations);
        }

        IEnumerable<QueryFilterOperation> MapQueryParts(
            IEnumerable<QueryParserResult> queryParts,
            IEnumerable<ParsedExpressionDefinition> filters)
        {
            return queryParts.Select(queryPart => new QueryFilterOperation
            {
                Filter = filters.SingleOrDefault(configFilter => string.Equals(configFilter.Name, queryPart.Property, StringComparison.InvariantCultureIgnoreCase)),
                OperationFactory = _config.OperationsMap[queryPart.Operation],
                LogicalOperationFactory = _config.LogicalOperationMap.TryGetValue(queryPart.LogicalOperation, out var factory) ? factory : null,
                QueryPart = queryPart
            });
        }
    }
}
