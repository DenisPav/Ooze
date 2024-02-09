using System.Reflection;

namespace Ooze.Typed.Expressions;

internal static class CommonMethods
{
    internal static readonly MethodInfo EnumerableContains = typeof(Enumerable)
        .GetMethods()
        .Where(x => x.Name == "Contains")
        .Single(x => x.GetParameters().Length == 2);
    internal static readonly MethodInfo StringStartsWith = typeof(string).GetMethod("StartsWith", [typeof(string)])!;
    internal static readonly MethodInfo StringEndsWith = typeof(string).GetMethod("EndsWith", [typeof(string)])!;
    internal static readonly MethodInfo CreateWrapperObject = typeof(BasicExpressions).GetMethod("CreateWrapperObject", BindingFlags.Static | BindingFlags.NonPublic)!;
}
