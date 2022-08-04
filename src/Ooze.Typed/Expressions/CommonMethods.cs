using System.Reflection;

namespace Ooze.Typed.Expressions;

internal static class CommonMethods
{
    internal static MethodInfo EnumerableContains = typeof(Enumerable)
        .GetMethods()
        .Where(x => x.Name == "Contains")
        .Single(x => x.GetParameters().Length == 2);
    internal static MethodInfo StringStartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
    internal static MethodInfo StringEndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
    internal static MethodInfo CreateWrapperObject = typeof(BasicExpressions).GetMethod("CreateWrapperObject", BindingFlags.Static | BindingFlags.NonPublic);
}
