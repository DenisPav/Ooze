using System.Reflection;

namespace Ooze.Typed.Sorters;

internal static class CommonMethods
{
    internal static MethodInfo OrderBy = typeof(Queryable).GetMethods()
        .Where(GetPredicate(nameof(OrderBy)))
        .Single();
    internal static MethodInfo OrderByDescending = typeof(Queryable).GetMethods()
        .Where(GetPredicate(nameof(OrderByDescending)))
        .Single();
    internal static MethodInfo ThenBy = typeof(Queryable).GetMethods()
        .Where(GetPredicate(nameof(ThenBy)))
        .Single();
    internal static MethodInfo ThenByDescending = typeof(Queryable).GetMethods()
        .Where(GetPredicate(nameof(ThenByDescending)))
        .Single();

    private static Func<MethodInfo, bool> GetPredicate(string methodName)
        => method => method.GetParameters().Count() == 2 && method.Name == methodName;
}
