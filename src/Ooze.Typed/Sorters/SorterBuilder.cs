using System.Linq.Expressions;

namespace Ooze.Typed.Sorters;

/// <inheritdoc/>
internal class SorterBuilder<TEntity, TSorters> : ISorterBuilder<TEntity, TSorters>
{
    private readonly IList<SortDefinition<TEntity, TSorters>> _sortDefinitions =
        new List<SortDefinition<TEntity, TSorters>>();

    public ISorterBuilder<TEntity, TSorters> SortBy<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TSorters, SortDirection?> sorterFunc,
        Func<TSorters, bool>? shouldRun = null)
    {
        shouldRun ??= sorters => sorterFunc(sorters) != null;
        _sortDefinitions.Add(new SortDefinition<TEntity, TSorters>
        {
            DataExpression = dataExpression,
            ShouldRun = shouldRun,
            GetSortDirection = sorters =>
            {
                var sortDirection = sorterFunc(sorters).GetValueOrDefault(SortDirection.Ascending);
                return sortDirection;
            }
        });

        return this;
    }

    public IEnumerable<SortDefinition<TEntity, TSorters>> Build()
        => _sortDefinitions;
}