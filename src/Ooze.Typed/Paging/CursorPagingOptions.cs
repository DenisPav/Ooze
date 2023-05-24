namespace Ooze.Typed.Paging;

/// <summary>
/// Class holding cursor paging related data
/// </summary>
/// <typeparam name="T"></typeparam>
public class CursorPagingOptions<T>
{
    /// <summary>
    /// After value for cursor paging
    /// </summary>
    public T? After { get; set; } = default;

    /// <summary>
    /// Page size for cursor paging
    /// </summary>
    public int Size { get; set; } = 100;
}