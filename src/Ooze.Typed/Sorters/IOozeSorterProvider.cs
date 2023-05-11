﻿namespace Ooze.Typed.Sorters;

/// <summary>
/// Sorter provider interface called internally by <see cref="IOozeTypedResolver"/>/
/// <see cref="IOozeTypedResolver{TEntity,TFilters,TSorters}"/> to fetch defined sorters for provided Entity type.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TSorters">Sorter implementation type</typeparam>
public interface IOozeSorterProvider<TEntity, TSorters>
{
    /// <summary>
    /// Method used for creation of <see cref="ISortDefinition{TEntity,TSorters}"/> definitions. These definitions are
    /// used in sorting process.
    /// </summary>
    /// <returns>Collection of sort definitions</returns>
    IEnumerable<ISortDefinition<TEntity, TSorters>> GetSorters();
}
