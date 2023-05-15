namespace Ooze.Typed.Filters;

/// <summary>
/// Class defining data needed for Range filter definitions
/// </summary>
/// <typeparam name="TType">Type of range properties</typeparam>
public class RangeFilter<TType>
{
    public TType? From { get; set; }
    public TType? To { get; set; }
}
