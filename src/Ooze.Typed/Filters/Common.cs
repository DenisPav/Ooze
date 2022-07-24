using System.Reflection;

namespace Ooze.Typed.Filters;

internal static class Common
{
    internal static MethodInfo EnumerableContains = typeof(Enumerable)
        .GetMethods()
        .Where(x => x.Name == "Contains")
        .Single(x => x.GetParameters().Length == 2);
}
