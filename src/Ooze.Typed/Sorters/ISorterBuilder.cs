using System.Linq.Expressions;

namespace Ooze.Typed.Sorters;

/// <summary>
/// Interface defining contract for sorter builder implementation
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TSorters">Sorter type</typeparam>
public interface ISorterBuilder<TEntity, TSorters>
{
    /// <summary>
    /// Creates a new sort definition fluently for specified entity property and sorter property
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="sorterFunc">Delegate targeting property of sorter class</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple sorter definitions</returns>
    ISorterBuilder<TEntity, TSorters> SortBy<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TSorters, SortDirection?> sorterFunc);

    /// <summary>
    /// Return collection of created sorter definitions
    /// </summary>
    /// <returns>Collection of sorter definitions</returns>
    IEnumerable<ISortDefinition<TEntity, TSorters>> Build();
}