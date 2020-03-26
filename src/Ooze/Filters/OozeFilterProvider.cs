using Ooze.Configuration;
using System;
using System.Linq;
using static Ooze.Expressions.OozeExpressionCreator;

namespace Ooze.Filters
{
    internal class OozeFilterProvider<TEntity> : IOozeFilterProvider<TEntity>
        where TEntity : class
    {
        readonly OozeConfiguration _configuration;

        public string Name { get; private set; }

        public OozeFilterProvider(
            OozeConfiguration configuration,
            string name)
        {
            _configuration = configuration;
            Name = name;
        }

        public IQueryable<TEntity> ApplyFilter(
            IQueryable<TEntity> query,
            FilterParserResult filter)
        {
            var entityConfiguration = _configuration.EntityConfigurations[typeof(TEntity)];
            var filterDefinition = entityConfiguration.Filters
                .SingleOrDefault(configFilter => string.Equals(configFilter.Name, filter.Property, StringComparison.InvariantCultureIgnoreCase));
            var callExpr = FilterExpression<TEntity>(entityConfiguration.Param, filter, filterDefinition, query.Expression, _configuration.OperationsMap[filter.Operation]);

            return query.Provider
                .CreateQuery<TEntity>(callExpr);
        }
    }
}
