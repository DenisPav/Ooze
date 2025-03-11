using System.Linq.Expressions;
using Ooze.Typed.Query.Exceptions;

namespace Ooze.Typed.Query.Filters;

internal class QueryFilterBuilder<TEntity> : IQueryFilterBuilder<TEntity>
{
    private readonly IList<QueryFilterDefinition<TEntity>> _filterDefinitions =
        new List<QueryFilterDefinition<TEntity>>();

    public IQueryFilterBuilder<TEntity> Add<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        string? name = null)
    {
        if (dataExpression.Body is not MemberExpression memberExpression)
        {
            throw new MemberExpressionException($"Query filter expression incorrect! Please check your filter provider! Expression used: [{dataExpression}]");
        }
        
        _filterDefinitions.Add(new QueryFilterDefinition<TEntity>
        {
            Name = string.IsNullOrEmpty(name)
                ? GetExpressionName(memberExpression)
                : name,
            TargetProperty = typeof(TEntity).GetProperty(GetExpressionName(memberExpression)),
        });

        return this;
    }

    private static string GetExpressionName(MemberExpression expression)
        => expression.Member.Name;

    public IEnumerable<QueryFilterDefinition<TEntity>> Build()
    {
        return _filterDefinitions;
    }
}