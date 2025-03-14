using System.Reflection;

namespace Ooze.Typed.Query.Filters;

public class QueryLanguageFilterDefinition<TEntity>
{
    public string? Name { get; init; }
    public PropertyInfo TargetProperty { get; init; }
}