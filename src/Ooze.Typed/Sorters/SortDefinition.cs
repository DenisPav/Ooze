using System.Linq.Expressions;

namespace Ooze.Typed.Sorters
{
    internal class SortDefinition<TEntity> : ISortDefinition<TEntity>
    {
        public LambdaExpression DataExpression { get; internal set; }
        public string PropertyName { get; internal set; }
    }
}
