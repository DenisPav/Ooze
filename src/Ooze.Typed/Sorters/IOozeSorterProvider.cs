using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Ooze.Typed.Sorters
{
    public interface IOozeSorterProvider<TEntity, TSorters>
    {
        IEnumerable<ISortDefinition<TEntity, TSorters>> GetSorters();
    }

    public interface ISortDefinition<TEntity, TSorters>
    { }

    internal class SortDefinition<TEntity, TSorters> : ISortDefinition<TEntity, TSorters>
    {
        public LambdaExpression DataExpression { get; internal set; }
        public Func<TSorters, bool> ShouldRun { get; internal set; }
        public Func<TSorters, SortDirection> GetSortDirection { get; internal set; }
    }

    public interface ISorterBuilder<TEntity, TSorters>
    {
        ISorterBuilder<TEntity, TSorters> Add<TProperty>(
            Expression<Func<TEntity, TProperty>> dataExpression,
            Func<TSorters, SortDirection?> sorterFunc);
        IEnumerable<ISortDefinition<TEntity, TSorters>> Build();
    }

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

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public static class Sorters
    {
        public static ISorterBuilder<TEntity, TSorters> CreateFor<TEntity, TSorters>()
            => new SorterBuilder<TEntity, TSorters>();
    }
}
