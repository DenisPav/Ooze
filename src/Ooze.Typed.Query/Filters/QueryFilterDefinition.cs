using System.Reflection;

namespace Ooze.Typed.Query.Filters;

public class QueryFilterDefinition<TEntity>
{
    public string? Name { get; init; }
    public PropertyInfo TargetProperty { get; init; }
}