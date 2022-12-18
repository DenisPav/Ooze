using System.Linq.Expressions;

namespace Ooze.Typed.Sorters
{
    public interface ISorterBuilder<TEntity, TSorters>
    {
        ISorterBuilder<TEntity, TSorters> Add<TProperty>(
            Expression<Func<TEntity, TProperty>> dataExpression,
            Func<TSorters, SortDirection?> sorterFunc);

        IEnumerable<ISortDefinition<TEntity, TSorters>> Build();
    }
}