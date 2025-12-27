using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Npgsql.Setup;

public record PostFilters(long? Id, string? Name, bool? Enabled, DateTime? Date);
public record PostLikeFilters(string LikeExpr);
public record PostPsqlFilters(string? ILikeFilter, string? SoundexFilter);
public record PostInFilters(IEnumerable<long> Ids, IEnumerable<string> Names, IEnumerable<DateTime> Dates);
public record PostRangeFilters(RangeFilter<long> Ids, RangeFilter<string> Names, RangeFilter<DateTime> Dates);