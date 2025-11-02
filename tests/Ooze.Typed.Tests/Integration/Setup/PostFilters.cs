using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Integration.Setup;

public record PostFilters(long? Id, string? Name, bool? Enabled, DateTime? Date);
public record PostGlobFilters(string GlobId);
public record PostLikeFilters(string LikeExpr);
public record PostInFilters(IEnumerable<long> Ids, IEnumerable<string> Names, IEnumerable<DateTime> Dates);
public record PostRangeFilters(RangeFilter<long> Ids, RangeFilter<string> Names, RangeFilter<DateTime> Dates);