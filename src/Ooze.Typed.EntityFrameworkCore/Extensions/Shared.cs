using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.EntityFrameworkCore.Extensions;

internal static class Shared
{
    public const string LikeMethod = nameof(DbFunctionsExtensions.Like);
    public static readonly Type DbFunctionsExtensionsType = typeof(DbFunctionsExtensions);
    public static readonly MemberExpression EfPropertyExpression = Expression.Property(null, typeof(EF), nameof(EF.Functions));
}