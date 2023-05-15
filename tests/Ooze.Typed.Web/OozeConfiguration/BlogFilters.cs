using Ooze.Typed.Filters;

public class BlogFilters
{
    public string Name { get; set; } = default!;
    public int? BlogId { get; set; } = default!;
    public IEnumerable<int> BlogIds { get; set; } = default!;
    public RangeFilter<int> BlogRange { get; set; } = default!;
    public bool? IsNameDate { get; set; } = default!;
}
