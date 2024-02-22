using System.Linq.Expressions;

namespace Ooze.Typed.Sorters.Async;

internal class AsyncSorterBuilder<TEntity, TSorters> : IAsyncSorterBuilder<TEntity, TSorters>
{
    private readonly IList<AsyncSortDefinition<TEntity, TSorters>> _sortDefinitions =
        new List<AsyncSortDefinition<TEntity, TSorters>>();

    public IAsyncSorterBuilder<TEntity, TSorters> SortBy<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TSorters, SortDirection?> sorterFunc,
        Func<TSorters, bool>? shouldRun = null)
    {
        shouldRun ??= sorters => sorterFunc(sorters) != null;
        _sortDefinitions.Add(new AsyncSortDefinition<TEntity, TSorters>
        {
            DataExpression = dataExpression,
            ShouldRun = sorters => Task.FromResult(shouldRun(sorters)),
            GetSortDirection = sorters =>
            {
                var sortDirection = sorterFunc(sorters);
                return Task.FromResult(sortDirection);
            }
        });

        return this;
    }

    public IAsyncSorterBuilder<TEntity, TSorters> SortByAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TSorters, Task<SortDirection?>> sorterFunc,
        Func<TSorters, Task<bool>>? shouldRun = null)
    {
        shouldRun ??= async sorters => await sorterFunc(sorters) != null;
        _sortDefinitions.Add(new AsyncSortDefinition<TEntity, TSorters>
        {
            DataExpression = dataExpression,
            ShouldRun = shouldRun,
            GetSortDirection = async sorters =>
            {
                var sortDirection = await sorterFunc(sorters);
                return sortDirection;
            }
        });

        return this;
    }

    public IEnumerable<AsyncSortDefinition<TEntity, TSorters>> Build()
        => _sortDefinitions;
}