namespace Ooze.Typed.Paging;

/// <summary>
/// Class holding paging related data
/// </summary>
public class PagingOptions
{
    /// <summary>
    /// Page size for pagination
    /// </summary>
    public int Size { get; set; } = 100;
    /// <summary>
    /// Current page for pagination
    /// </summary>
    public int Page { get; set; } = 0;
}
