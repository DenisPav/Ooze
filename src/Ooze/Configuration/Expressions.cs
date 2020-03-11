using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Ooze.Configuration
{
    internal class Expressions
    {
        internal static MethodInfo StringContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        internal static MethodInfo StringStartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        internal static MethodInfo StringEndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        internal static Expression Contains(Expression left, Expression right) => Call(left, StringContains, right);
        internal static Expression StartsWith(Expression left, Expression right) => Call(left, StringStartsWith, right);
        internal static Expression EndsWith(Expression left, Expression right) => Call(left, StringEndsWith, right);

    }
}
