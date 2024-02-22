using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Ooze.Typed.Expressions;

namespace Ooze.Typed.Sorters;

internal class SorterHandler<TEntity, TSorters>(
    IEnumerable<ISorterProvider<TEntity, TSorters>> sortProviders,
    ILogger<SorterHandler<TEntity, TSorters>> log)
    : ISorterHandler<TEntity, TSorters>
{
    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        IEnumerable<TSorters> sorters)
    {
        log.LogDebug("Processing available sorters!");

        if (query == null) throw new ArgumentNullException(nameof(query));
        var sortDefinitions = sortProviders.SelectMany(provider => provider.GetSorters())
            .ToList();

        foreach (var sorter in sorters)
        {
            var sortDefinition = sortDefinitions.SingleOrDefault(definition => definition.ShouldRun(sorter));
            if (sortDefinition is null)
                continue;

            var sorterType = BasicExpressions.GetMemberExpression(sortDefinition.DataExpression.Body).Type;
            var direction = sortDefinition.GetSortDirection(sorter);
            MethodInfo? method;

            if (query!.Expression.Type == typeof(IOrderedQueryable<TEntity>))
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