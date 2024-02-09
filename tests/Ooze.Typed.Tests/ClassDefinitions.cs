using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests;

public record Blog(int Id, string Name, int NumberOfComments);
public record BlogFilters(int? IdFilter, string NameFilter, int? NumberOfCommentsFilter);
public record BlogSorters(SortDirection? IdSort, SortDirection? NameSort, SortDirection? NumberOfCommentsSort);
