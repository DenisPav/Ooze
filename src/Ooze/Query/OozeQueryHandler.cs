using Ooze.Configuration;
using Ooze.Parsers;
using Superpower;
using System.Linq;

namespace Ooze.Query
{
    internal class OozeQueryHandler : IOozeQueryHandler
    {
        readonly OozeConfiguration _config;

        public OozeQueryHandler(
            OozeConfiguration config)
        {
            _config = config;
        }

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string modelQuery) where TEntity : class
        {
            var entityType = typeof(TEntity);
            var entityConfiguration = _config.EntityConfigurations[entityType];
            var parser = CreateParser(entityConfiguration);

            var parsed = parser.TryParse(modelQuery);
            if (parsed.HasValue)
            {
                query = OozeQueryableCreator.ForQuery(query, entityConfiguration, parsed.Value, _config.OperationsMap, _config.LogicalOperationMap);
            }

            return query;
        }

        TextParser<QueryParserResult[]> CreateParser(
            OozeEntityConfiguration entityConfiguration)
        {
            var filterNames = entityConfiguration.Filters
                .Select(filter => filter.Name);
            var operations = _config.OperationsMap
                .Keys;
            var logicalOperations = _config.LogicalOperationMap
                .Keys;

            return OozeParserCreator.QueryParser(filterNames, operations, logicalOperations);
        }
    }
}
