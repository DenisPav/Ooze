using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Npgsql;

public record PostFilters(
    long? PostId = null,
    long? NotEqualPostId = null,
    long? GreaterThanPostId = null,
    long? LessThanPostId = null,
    IEnumerable<long>? IdIn = null,
    IEnumerable<long>? IdNotIn = null,
    RangeFilter<long>? IdRange = null,
    RangeFilter<long>? IdOutOfRange = null,
    string? NameStartsWith = null,
    string? NameDoesntWith = null,
    string? NameEndsWith = null,
    string? NameDoesntEndWith = null,
    string? NameLikeFilter = null,
    string? NameSoundexEqual = null
);