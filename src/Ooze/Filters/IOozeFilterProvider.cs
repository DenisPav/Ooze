using Ooze.Configuration;
using Ooze.Expressions;
using Ooze.Sorters;
using System;
using System.Linq;
using System.Linq.Expressions;

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

    internal class OozeSorterProvider<TEntity> : IOozeSorterProvider<TEntity>
        where TEntity : class
    {
        readonly Expression _expression;
        readonly Type _propType;

        public string Name { get; private set; }

        public OozeSorterProvider(
            string name,
            Expression expression,
            Type propType)
        {
            Name = name;
            _expression = expression;
            _propType = propType;
        }

        public IQueryable<TEntity> ApplySorter(IQueryable<TEntity> query, bool ascending)
        {
            var sortExpression = OozeExpressionCreator.SortExpression<TEntity>(query.Expression, _expression, _propType, ascending, true);

            return query.Provider
                .CreateQuery<TEntity>(sortExpression);
        }

        public IQueryable<TEntity> ThenApplySorter(IOrderedQueryable<TEntity> query, bool ascending)
        {
            var sortExpression = OozeExpressionCreator.SortExpression<TEntity>(query.Expression, _expression, _propType, ascending, false);

            return query.Provider
                .CreateQuery<TEntity>(sortExpression);
        }
    }
}
