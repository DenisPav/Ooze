using Ooze.Configuration;
using Ooze.Filters;
using Superpower;
using System.Linq;

namespace Ooze.Query
{
    internal interface IOozeQueryHandler
    {
        IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string modelQuery)
            where TEntity : class;
    }

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
            var parser = OozeParserCreator.QueryParser(entityConfiguration);

            var parsed = parser.TryParse(modelQuery);
            if (parsed.HasValue)
            {
                var queryParts = parsed.Value;
                query = OozeQueryableCreator.ForQuery(query, entityConfiguration, queryParts, _config.OperationsMap, _config.LogicalOperationMap);
            }

            return query;
        }
    }
}
