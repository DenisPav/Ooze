using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.EntityFrameworkCore.Sqlite.Extensions;

internal static class Shared
{
    public const string GlobMethod = nameof(SqliteDbFunctionsExtensions.Glob);
    public static readonly Type DbFunctionsExtensionsType = typeof(SqliteDbFunctionsExtensions);
    public static readonly MemberExpression EfPropertyExpression = Expression.Property(null, typeof(EF), nameof(EF.Functions));
}