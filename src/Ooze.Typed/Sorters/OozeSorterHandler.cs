using Microsoft.Extensions.Logging;
using Ooze.Typed.Expressions;

namespace Ooze.Typed.Sorters;

internal class OozeSorterHandler<TEntity, TSorters> : IOozeSorterHandler<TEntity, TSorters>
{
    private readonly IEnumerable<IOozeSorterProvider<TEntity, TSorters>> _sortProviders;
    private readonly ILogger<OozeSorterHandler<TEntity, TSorters>> _log;

    public OozeSorterHandler(
        IEnumerable<IOozeSorterProvider<TEntity, TSorters>> sortProviders,
        ILogger<OozeSorterHandler<TEntity, TSorters>> log)
    {
        _sortProviders = sortProviders;
        _log = log;
    }

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters)
    {
        _log.LogDebug("Processing available sorters!");

        if (query == null) throw new ArgumentNullException(nameof(query));
        var sortDefinitions = _sortProviders.SelectMany(provider => provider.GetSorters())
            .Cast<SortDefinition<TEntity, TSorters>>()
            .ToList();

        foreach (var sorter in sorters)
        {
            var sortDefinition = sortDefinitions.SingleOrDefault(definition => definition.ShouldRun(sorter));
            if (sortDefinition is null)
                continue;

            var sorterType = BasicExpressions.GetMemberExpression(sortDefinition.DataExpression.Body).Type;
            var direction = sortDefinition.GetSortDirection(sorter);

            if (query!.Expression.Type == typeof(IOrderedQueryable<TEntity>))
            {
                query = (direction == SortDirection.Ascending
                    ? CommonMethods.ThenBy
                        .MakeGenericMethod(typeof(TEntity), sorterType)
                        .Invoke(null, new object[] { query, sortDefinition.DataExpression }) as IQueryable<TEntity>
                    : CommonMethods.ThenByDescending
                        .MakeGenericMethod(typeof(TEntity), sorterType)
                        .Invoke(null, new object[] { query, sortDefinition.DataExpression }) as IQueryable<TEntity>)!;

                _log.LogDebug("Applying sorter: [{@sorter}]", sortDefinition.DataExpression);
            }
            else
            {
                query = (direction == SortDirection.Ascending
                    ? CommonMethods.OrderBy
                        .MakeGenericMethod(typeof(TEntity), sorterType)
                        .Invoke(null, new object[] { query, sortDefinition.DataExpression }) as IQueryable<TEntity>
                    : CommonMethods.OrderByDescending
                        .MakeGenericMethod(typeof(TEntity), sorterType)
                        .Invoke(null, new object[] { query, sortDefinition.DataExpression }) as IQueryable<TEntity>)!;

                _log.LogDebug("Applying sorter: [{@sorter}]", sortDefinition.DataExpression);
            }
        }

        return query;
    }
}
