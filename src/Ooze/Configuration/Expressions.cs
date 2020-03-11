using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Ooze.Configuration
{
    internal class Expressions
    {
        internal static MethodInfo StringContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        internal static Expression Contains(Expression left, Expression right) => Call(left, StringContains, right);
    }
}
