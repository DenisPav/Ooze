namespace Ooze.Typed.Filters;

/// <summary>
/// Class defining data needed for Range filter definitions
/// </summary>
/// <typeparam name="TType">Type of range properties</typeparam>
public class RangeFilter<TType>
{
    /// <summary>
    /// Property defining start of range
    /// </summary>
    public TType? From { get; set; }
    /// <summary>
    /// Property defining end of range
    /// </summary>
    public TType? To { get; set; }
}
