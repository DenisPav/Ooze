using System.Linq.Expressions;
using Ooze.Typed.Query.Exceptions;

namespace Ooze.Typed.Query.Filters;

internal class QueryFilterBuilder<TEntity> : IQueryFilterBuilder<TEntity>
{
    private readonly IList<QueryFilterDefinition<TEntity>> _filterDefinitions =
        new List<QueryFilterDefinition<TEntity>>();

    public IQueryFilterBuilder<TEntity> Add<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        string name = null)
    {
        _filterDefinitions.Add(new QueryFilterDefinition<TEntity>
        {
            Name = string.IsNullOrEmpty(name)
                ? GetExpressionName(dataExpression)
                : name,
            TargetProperty = typeof(TEntity).GetProperty(GetExpressionName(dataExpression)),
            DataExpression = dataExpression
        });

        return this;
    }

    private static string GetExpressionName<TTarget>(Expression<Func<TEntity, TTarget>> expression)
    {
        if (expression.Body is not MemberExpression memberExpression)
        {
            throw new MemberExpressionException($"Query filter expression incorrect! Please check your filter provider! Expression used: [{expression}]");
        }

        var memberName = memberExpression.Member.Name;
        return memberName;
    }

    public IEnumerable<IQueryFilterDefinition<TEntity>> Build()
    {
        return _filterDefinitions;
    }
}