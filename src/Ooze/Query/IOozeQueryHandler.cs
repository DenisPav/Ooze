using Ooze.Configuration;
using Ooze.Filters;
using Superpower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            var parsed = parser.Parse(modelQuery);

            return query;
        }
    }
}
