using System.Linq.Expressions;

namespace Ooze.Typed.Query.Filters;

public interface IQueryLanguageFilterBuilder<TEntity>
{
    IQueryLanguageFilterBuilder<TEntity> Add<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        string? name = null);
    IEnumerable<QueryLanguageFilterDefinition<TEntity>> Build();
}