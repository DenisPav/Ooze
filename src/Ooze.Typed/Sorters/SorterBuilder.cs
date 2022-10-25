using System.Linq.Expressions;
using Ooze.Typed.Expressions;

namespace Ooze.Typed.Sorters
{
    internal class SorterBuilder<TEntity> : ISorterBuilder<TEntity>
    {
        readonly IList<SortDefinition<TEntity>> _sortDefinitions = new List<SortDefinition<TEntity>>();

        public ISorterBuilder<TEntity> Add<TProperty>(Expression<Func<TEntity, TProperty>> dataExpression)
        {
            _sortDefinitions.Add(new SortDefinition<TEntity>
            {
                DataExpression = dataExpression,
                PropertyName = BasicExpressions.GetExpressionName(dataExpression)
            });

            return this;
        }

        public IEnumerable<ISortDefinition<TEntity>> Build()
            => _sortDefinitions;
    }
}
