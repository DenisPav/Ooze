using System.Linq.Expressions;

namespace Ooze.Typed.Filters.Async;

public interface IAsyncFilterBuilder<TEntity, TFilter> : IFilters<TEntity, TFilter, IAsyncFilterBuilder<TEntity, TFilter>>
{
    /// <summary>
    /// Fluently creates custom async filter definition
    /// </summary>
    /// <param name="shouldRun">Delegate showing if the filter should execute</param>
    /// <param name="filterExpressionFactory">Factory for creation of filter Expression</param>
    /// <returns>Instance of builder for fluent building of multiple filter definitions</returns>
    IAsyncFilterBuilder<TEntity, TFilter> AddAsync(
        Func<TFilter, Task<bool>> shouldRun,
        Func<TFilter, Task<Expression<Func<TEntity, bool>>>> filterExpressionFactory);

    /// <summary>
    /// Return collection of created async filter definitions
    /// </summary>
    /// <returns>Collection of filter definitions</returns>
    IEnumerable<AsyncFilterDefinition<TEntity, TFilter>> Build();
}