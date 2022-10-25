using Ooze.Typed.Expressions;

namespace Ooze.Typed.Sorters
{
    internal class OozeSorterHandler<TEntity> : IOozeSorterHandler<TEntity>
    {
        private readonly IEnumerable<IOozeSorterProvider<TEntity>> _sortProviders;

        public OozeSorterHandler(
            IEnumerable<IOozeSorterProvider<TEntity>> sortProviders)
        {
            _sortProviders = sortProviders;
        }

        public IQueryable<TEntity> Apply(
            IQueryable<TEntity> query,
            IEnumerable<Sorter> sorters)
        {
            var sortersDictionary = sorters.ToDictionary(x => x.Name, x => x.Direction);
            var validSorters = _sortProviders.SelectMany(provider => provider.GetSorters())
                .Cast<SortDefinition<TEntity>>()
                .Where(sorterDefinition => sortersDictionary.ContainsKey(sorterDefinition.PropertyName))
                .ToDictionary(sorterDefinition => sorterDefinition.PropertyName);
            
            foreach (var sorter in sorters)
            {
                var sortDefinition = validSorters[sorter.Name];
                var sorterType = BasicExpressions.GetMemberExpression(sortDefinition.DataExpression.Body).Type;
                var direction = sortersDictionary[sortDefinition.PropertyName];

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
