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
            var validSorters = _sortProviders.SelectMany(provider => provider.GetSorters())
                .Cast<SortDefinition<TEntity>>()
                .ToDictionary(sorterDefinition => sorterDefinition.PropertyName);
            var sortersDictionary = sorters.Where(sorter => validSorters.ContainsKey(sorter.Name))
                .ToDictionary(x => x.Name, x => x.Direction);
            
            foreach (var sorter in sortersDictionary.Keys)
            {
                var sortDefinition = validSorters[sorter];
                var sorterType = BasicExpressions.GetMemberExpression(sortDefinition.DataExpression.Body).Type;
                var direction = sortersDictionary[sorter];

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
