﻿using System.Linq.Expressions;

namespace Ooze.Typed.Sorters.Async;

/// <summary>
/// Interface defining contract for async sorter builder implementation
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TSorters">Sorter type</typeparam>
public interface IAsyncSorterBuilder<TEntity, TSorters>
{
    /// <summary>
    /// Creates a new sort definition fluently for specified entity property and sorter property
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="sorterFunc">Delegate targeting property of sorter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if sorter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple sorter definitions</returns>
    IAsyncSorterBuilder<TEntity, TSorters> SortBy<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TSorters, SortDirection?> sorterFunc,
        Func<TSorters, bool>? shouldRun = null);
    
    /// <summary>
    /// Creates a new async sort definition fluently for specified entity property and sorter property
    /// </summary>
    /// <param name="dataExpression">Expression targeting property of specified entity class</param>
    /// <param name="sorterFunc">Delegate targeting property of sorter class</param>
    /// <param name="shouldRun">Delegate returning bool value which denotes if sorter should be applied</param>
    /// <typeparam name="TProperty">Target type of entity property</typeparam>
    /// <returns>Instance of builder for fluent building of multiple sorter definitions</returns>
    IAsyncSorterBuilder<TEntity, TSorters> SortByAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TSorters, Task<SortDirection?>> sorterFunc,
        Func<TSorters, Task<bool>>? shouldRun = null);

    /// <summary>
    /// Return collection of created sorter definitions
    /// </summary>
    /// <returns>Collection of sorter definitions</returns>
    IEnumerable<AsyncSortDefinition<TEntity, TSorters>> Build();
}