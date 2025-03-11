using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Ooze.Typed.Paging;

namespace Ooze.Typed.Query;

public interface IQueryOperationResolver
    : IOperationResolver
{
    IQueryable<TEntity> Query<TEntity>(
        IQueryable<TEntity> queryable,
        string? query);
}

public interface IQueryOperationResolver<TEntity, TFilters, TSorterts>
    : IOperationResolver<TEntity, TFilters, TSorterts>
{
    IQueryOperationResolver<TEntity, TFilters, TSorterts> Query(string query);
}