using Ooze.Typed.Expressions;

namespace Ooze.Typed.Sorters;

internal class OozeSorterHandler<TEntity, TSorter> : IOozeSorterHandler<TEntity, TSorter>
{
    private readonly IEnumerable<IOozeSorterProvider<TEntity, TSorter>> _sortProviders;

    public OozeSorterHandler(
        IEnumerable<IOozeSorterProvider<TEntity, TSorter>> sortProviders)
    {
        _sortProviders = sortProviders;
    }

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorter> sorters)
    {
        var sortDefinitions = _sortProviders.SelectMany(provider => provider.GetSorters())
            .Cast<SortDefinition<TEntity, TSorter>>()
            .ToList();

        foreach (var sorter in sorters)
        {
            var sortDefinition = sortDefinitions.SingleOrDefault(definition => definition.ShouldRun(sorter));
            if (sortDefinition is null)
                continue;

            var sorterType = BasicExpressions.GetMemberExpression(sortDefinition.DataExpression.Body).Type;
            var direction = sortDefinition.GetSortDirection(sorter);

            if (query.Expression.Type == typeof(IOrderedQueryable<TEntity>))
            {
                query = direction == SortDirection.Ascending
                    ? CommonMethods.ThenBy
                        .MakeGenericMethod(typeof(TEntity), sorterType)
                        .Invoke(null, new object[] { query, sortDefinition.DataExpression }) as IQueryable<TEntity>
                    : CommonMethods.ThenByDescending
                        .MakeGenericMethod(typeof(TEntity), sorterType)
                        .Invoke(null, new object[] { query, sortDefinition.DataExpression }) as IQueryable<TEntity>;
            }
            else
            {
                query = direction == SortDirection.Ascending
                    ? CommonMethods.OrderBy
                        .MakeGenericMethod(typeof(TEntity), sorterType)
                        .Invoke(null, new object[] { query, sortDefinition.DataExpression }) as IQueryable<TEntity>
                    : CommonMethods.OrderByDescending
                        .MakeGenericMethod(typeof(TEntity), sorterType)
                        .Invoke(null, new object[] { query, sortDefinition.DataExpression }) as IQueryable<TEntity>;
            }
        }

        return query;
    }
}
