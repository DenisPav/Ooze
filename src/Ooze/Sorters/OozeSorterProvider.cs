using System;
using System.Linq;
using System.Linq.Expressions;
using static Ooze.Expressions.OozeExpressionCreator;

namespace Ooze.Sorters
{
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

        public IQueryable<TEntity> ApplySorter(
            IQueryable<TEntity> query,
            bool ascending)
        {
            var sortExpression = CreateSortExpression(query, ascending, true);
            return query.Provider
                .CreateQuery<TEntity>(sortExpression);
        }

        public IQueryable<TEntity> ThenApplySorter(
            IOrderedQueryable<TEntity> query,
            bool ascending)
        {
            var sortExpression = CreateSortExpression(query, ascending, false);
            return query.Provider
                .CreateQuery<TEntity>(sortExpression);
        }

        MethodCallExpression CreateSortExpression(
            IQueryable<TEntity> query,
            bool ascending,
            bool isFirst)
            => SortExpression<TEntity>(query.Expression, _expression, _propType, ascending, isFirst);
    }
}
