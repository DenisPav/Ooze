using System.Linq.Expressions;

namespace Ooze.Typed.Query.Filters;

internal class QueryFilterDefinition<TEntity> : IQueryFilterDefinition<TEntity>
{
    public string Name { get; set; }
    public LambdaExpression DataExpression { get; set; }
}