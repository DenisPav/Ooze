using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.EntityFrameworkCore.Npgsql.Extensions;

internal static class Shared
{
    // ReSharper disable once InconsistentNaming
    public const string ILikeMethod = nameof(NpgsqlDbFunctionsExtensions.ILike);
    public const string FuzzyStringMatchSoundexMethod = nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchSoundex);
    public static readonly Type DbFunctionsExtensionsType = typeof(NpgsqlDbFunctionsExtensions);
    public static readonly Type FuzzyStringMatchDbFunctionsExtensionsType = typeof(NpgsqlFuzzyStringMatchDbFunctionsExtensions);
    public static readonly MemberExpression EfPropertyExpression = Expression.Property(null, typeof(EF), nameof(EF.Functions));
}