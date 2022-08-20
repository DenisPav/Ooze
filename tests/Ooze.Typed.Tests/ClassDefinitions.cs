using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests;

public record class Blog(int Id, string Name, int NumberOfComments);
public record class BlogFilters(int? IdFilter, string NameFilter, int? NumberOfCommentsFilter);
public record class BlogSorters(SortDirection? IdSort, SortDirection? NameSort, SortDirection? NumberOfCommentsSort);
