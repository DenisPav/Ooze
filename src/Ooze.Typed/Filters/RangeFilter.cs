namespace Ooze.Typed.Filters;

public class RangeFilter<TType>
{
    public TType? From { get; set; }
    public TType? To { get; set; }
}
