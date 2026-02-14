using Ooze.Typed.Sorters;

namespace Ooze.Typed.Query.Tests;

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NumberOfComments { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record BlogFilters(int? IdFilter, string NameFilter, int? NumberOfCommentsFilter);

public record BlogSorters(SortDirection? IdSort, SortDirection? NameSort, SortDirection? NumberOfCommentsSort);