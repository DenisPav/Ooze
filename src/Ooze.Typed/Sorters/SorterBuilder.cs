﻿using System.Linq.Expressions;

namespace Ooze.Typed.Sorters
{
    internal class SorterBuilder<TEntity, TSorters> : ISorterBuilder<TEntity, TSorters>
    {
        readonly IList<SortDefinition<TEntity, TSorters>> _sortDefinitions = new List<SortDefinition<TEntity, TSorters>>();

        public ISorterBuilder<TEntity, TSorters> Add<TProperty>(
            Expression<Func<TEntity, TProperty>> dataExpression,
            Func<TSorters, SortDirection?> sorterFunc)
        {
            _sortDefinitions.Add(new SortDefinition<TEntity, TSorters>
            {
                DataExpression = dataExpression,
                ShouldRun = sorters => sorterFunc(sorters) != null,
                GetSortDirection = sorters =>
                {
                    var sortDirection = sorterFunc(sorters).GetValueOrDefault(SortDirection.Ascending);
                    return sortDirection;
                }
            });

            return this;
        }

        public IEnumerable<ISortDefinition<TEntity, TSorters>> Build()
            => _sortDefinitions;
    }
}