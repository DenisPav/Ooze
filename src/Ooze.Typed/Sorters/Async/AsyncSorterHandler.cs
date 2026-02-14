using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Ooze.Typed.Expressions;

namespace Ooze.Typed.Sorters.Async;

internal class AsyncSorterHandler<TEntity, TSorters>(
    IEnumerable<IAsyncSorterProvider<TEntity, TSorters>> sortProviders,
    ILogger<AsyncSorterHandler<TEntity, TSorters>> log)
    : IAsyncSorterHandler<TEntity, TSorters>
{
    public async ValueTask<IQueryable<TEntity>> ApplyAsync(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters)
    {
        log.LogDebug("Processing available sorters!");

        if (query == null) throw new ArgumentNullException(nameof(query));
        var sortDefinitions = new List<AsyncSortDefinition<TEntity, TSorters>>();
        foreach (var provider in sortProviders)
        {
            var definitions = await provider.GetSortersAsync()
                .ConfigureAwait(false);
            sortDefinitions.AddRange(definitions);
        }

        foreach (var sorter in sorters)
        {
            async Task<AsyncSortDefinition<TEntity, TSorters>?> GetSorterDefinition()
            {
                foreach (var sorterDefinition in sortDefinitions)
                {
                    var shouldRun = await sorterDefinition.ShouldRun(sorter)
                        .ConfigureAwait(false);
                    if (shouldRun == true)
                        return sorterDefinition;
                }

                return null;
            }

            var sortDefinition = await GetSorterDefinition().ConfigureAwait(false);
            if (sortDefinition is null)
                continue;

            var sorterType = BasicExpressions.GetMemberExpression(sortDefinition.DataExpression.Body).Type;
            var direction = await sortDefinition.GetSortDirection(sorter)
                .ConfigureAwait(false);
            MethodInfo? method;

            if (query.Expression.Type == typeof(IOrderedQueryable<TEntity>))
            {
                method = direction == SortDirection.Ascending
                    ? CommonMethods.ThenBy
                    : CommonMethods.ThenByDescending;
            }
            else
            {
                method = direction == SortDirection.Ascending
                    ? CommonMethods.OrderBy
                    : CommonMethods.OrderByDescending;
            }

            log.LogDebug("Applying sorter: [{@sorter}]", sortDefinition.DataExpression);
            query = CreateSortedQueryable(query, method, sorterType, sortDefinition.DataExpression);
        }

        return query;
    }

    private static IQueryable<TEntity> CreateSortedQueryable(
        IQueryable<TEntity> query,
        MethodInfo method,
        Type sorterType,
        LambdaExpression dataExpression)
    {
        return (method.MakeGenericMethod(typeof(TEntity), sorterType)
            .Invoke(null, [query, dataExpression]) as IQueryable<TEntity>)!;
    }
}