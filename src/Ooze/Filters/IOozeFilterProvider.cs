using Ooze.Configuration;
using Ooze.Expressions;
using System;
using System.Linq;

namespace Ooze.Filters
{
    public interface IOozeFilterProvider<TEntity> : IOozeProvider
        where TEntity : class
    {
        IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, FilterParserResult filter);
    }

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

        public IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, FilterParserResult filter)
        {
            var entityConfiguration = _configuration.EntityConfigurations[typeof(TEntity)];
            var filterDefinition = entityConfiguration.Filters
                .SingleOrDefault(configFilter => string.Equals(configFilter.Name, filter.Property, StringComparison.InvariantCultureIgnoreCase));
            var callExpr = OozeExpressionCreator.FilterExpression<TEntity>(entityConfiguration, filter, filterDefinition, query.Expression, _configuration.OperationsMap[filter.Operation]);

            return query.Provider
                .CreateQuery<TEntity>(callExpr);
        }
    }
}
