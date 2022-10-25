using System.Linq.Expressions;

namespace Ooze.Typed.Sorters
{
    public interface ISorterBuilder<TEntity>
    {
        ISorterBuilder<TEntity> Add<TProperty>(Expression<Func<TEntity, TProperty>> dataExpression);
        IEnumerable<ISortDefinition<TEntity>> Build();
    }
}
