using Ooze.Typed.Filters;
using System.Linq.Expressions;

namespace Ooze.Typed.Sorters
{
    internal interface IOozeSorterHandler<TEntity, TSorters>
    {
        IQueryable<TEntity> Apply(
            IQueryable<TEntity> query,
            TSorters sorters);
    }

    internal class OozeSorterHandler<TEntity, TSorters> : IOozeSorterHandler<TEntity, TSorters>
    {
        private readonly IEnumerable<IOozeSorterProvider<TEntity, TSorters>> _sortProviders;

        public OozeSorterHandler(
            IEnumerable<IOozeSorterProvider<TEntity, TSorters>> sortProviders)
        {
            _sortProviders = sortProviders;
        }

        public IQueryable<TEntity> Apply(
            IQueryable<TEntity> query,
            TSorters sorters)
        {
            var validSorters = _sortProviders.SelectMany(provider => provider.GetSorters())
                .Cast<SortDefinition<TEntity, TSorters>>()
                .Where(sorter => sorter.ShouldRun(sorters));


            foreach (var sortDefinition in validSorters)
            {
                var sorterType = BasicExpressions.GetMemberExpression(sortDefinition.DataExpression.Body).Type;
                var direction = sortDefinition.GetSortDirection(sorters);

                if(query.Expression.Type == typeof(IOrderedQueryable<TEntity>))
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
}
