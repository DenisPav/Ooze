using System.Linq.Expressions;
using System.Reflection;

namespace Ooze.Typed.Query.Filters;

internal class QueryFilterDefinition<TEntity> : IQueryFilterDefinition<TEntity>
{
    public string Name { get; init; }
    public LambdaExpression DataExpression { get; set; }
    public PropertyInfo TargetProperty { get; set; }
}