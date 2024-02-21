using System.Linq.Expressions;

namespace Ooze.Typed.Sorters.Async;

public interface IAsyncSorterBuilder<TEntity, TSorters>
{
    IAsyncSorterBuilder<TEntity, TSorters> SortBy<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TSorters, SortDirection?> sorterFunc,
        Func<TSorters, bool>? shouldRun = null);
    
    IAsyncSorterBuilder<TEntity, TSorters> SortByAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TSorters, Task<SortDirection?>> sorterFunc,
        Func<TSorters, Task<bool>>? shouldRun = null);

    IEnumerable<AsyncSortDefinition<TEntity, TSorters>> Build();
}